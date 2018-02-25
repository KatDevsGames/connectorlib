/*
 * Copyright 2018 Equilateral IT
 *
 * ConnectorLib is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ConnectorLib is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with ConnectorLib.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ConnectorLib
{
    /// <summary>
    /// Implements ISNESConnector with a LUA script run in an emulator.
    /// </summary>
    public class LuaConnector : ISNESConnector
    {
        /// <summary>
        /// Backing storage for NextID.
        /// </summary>
        private static uint _next_id = 0;

        /// <summary>
        /// An auto-incrementing ID for convenience purposes.
        /// </summary>
        private static uint NextID { get { unchecked { return _next_id++; } } }

        /// <summary>
        /// The listener for the LUA script to connect to.
        /// </summary>
        [NotNull]
        private readonly TcpListener _listener;

        /// <summary>
        /// The TcpClient object representing the LUA script data socket.
        /// </summary>
        [CanBeNull]
        private volatile TcpClient _client;

        /// <summary>
        /// The LUA script socket stream.
        /// </summary>
        [CanBeNull]
        private volatile NetworkStream _stream;

        /// <summary>
        /// True if the connector object is tearing down, otherwise false.
        /// </summary>
        private volatile bool _quitting = false;

        /// <summary>
        /// A callback function for diagnostic messages.
        /// </summary>
        [NotNull]
        private readonly Action<string> _message_handler;

        /// <summary>
        /// The response wait handles for each request.
        /// </summary>
        [NotNull]
        private readonly Dictionary<uint, ManualResetEvent> _response_events = new Dictionary<uint, ManualResetEvent>();

        /// <summary>
        /// The response values from reads.
        /// </summary>
        [NotNull]
        private readonly Dictionary<uint, uint?> _response_values = new Dictionary<uint, uint?>();

        /// <summary>
        /// True if the LUA script is connected to the socket, otherwise false.
        /// </summary>
        public bool Connected => _client.IsConnected();

        /// <summary>
        /// Disposes of socket resources.
        /// </summary>
        public void Dispose()
        {
            _quitting = true;
            try { _listener.Stop(); }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
            try { _client?.Dispose(); }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }

        ~LuaConnector() => Dispose();

        /// <summary>
        /// Creates a new LuaConnector object with a specified endpoint.
        /// </summary>
        /// <param name="endpoint">The IP endpoint that the connector should listen on.</param>
        public LuaConnector([CanBeNull] IPAddress endpoint = null) : this(s => { }, endpoint) { }

        /// <summary>
        /// Creates a new LuaConnector object with a specified endpoint.
        /// </summary>
        /// <param name="messageHandler">A callback for diagnostic messages.</param>
        /// <param name="endpoint">The IP endpoint that the connector should listen on.</param>
        public LuaConnector([CanBeNull] Action<string> messageHandler, [CanBeNull] IPAddress endpoint = null)
        {
            if (endpoint == null) { endpoint = IPAddress.Loopback; }
            // ReSharper disable once AssignNullToNotNullAttribute
            _listener = new TcpListener(endpoint, 43884);
            _message_handler = messageHandler ?? (s => { });

            _listener.Start();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        _message_handler("LuaConnector is listening.");
                        try { _client = _listener.AcceptTcpClient(); }
                        catch (SocketException)
                        {
                            _message_handler("LuaConnector listener caught a SocketException.");
                            if (_quitting)
                            {
                                _message_handler("LuaConnector listener is quitting.");
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                        _message_handler("LuaConnector listener is connected.");
                        _stream = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        _stream = _client?.GetStream();
                        _message_handler("LuaConnector listener socket established successfully.");
                        if (_quitting)
                        {
                            _message_handler("LuaConnector listener is quitting.");
                            return;
                        }
                    }
                }
                catch
                {
                    if (_quitting)
                    {
                        _message_handler("LuaConnector block processor is quitting.");
                        return;
                    }
                    Thread.Sleep(5000);
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                List<byte> buffer = new List<byte>();
                while (!_quitting)
                {
                    try
                    {
                        if ((!_client.IsConnected()) || (_stream == null))
                        {
                            if (_quitting)
                            {
                                _message_handler("LuaConnector block processor is quitting.");
                                return;
                            }
                            buffer.Clear();
                            Thread.Sleep(1000);
                            continue;
                        }
                        int next = _stream.ReadByte();
                        if (next == -1)
                        {
                            if (_quitting)
                            {
                                _message_handler("LuaConnector block processor is quitting.");
                                return;
                            }
                            buffer.Clear();
                            Thread.Sleep(1000);
                            continue;
                        }
                        buffer.Add((byte)next);
                        if (next == 0)
                        {
                            try { ProcessBlock(Encoding.UTF8.GetString(buffer.ToArray())); }
                            catch(Exception e) { _message_handler($"LuaConnector block processor caught an exception. [{e.GetType().Name}]"); }
                            buffer.Clear();
                        }
                    }
                    catch
                    {
                        if (_quitting)
                        {
                            _message_handler("LuaConnector block processor is quitting.");
                            return;
                        }
                        buffer.Clear();
                        Thread.Sleep(5000);
                    }
                }
                _message_handler("LuaConnector block processor is quitting.");
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                while (!_quitting)
                {
                    try { KeepAlive(); }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch { }
                    Thread.Sleep(2500);
                }
                _message_handler("LuaConnector keepalive is quitting.");
            }, TaskCreationOptions.LongRunning);
        }

        [DebuggerStepThrough]
        private void KeepAlive() => WriteBlock(new LuaBlock(NextID, LuaBlock.CommandType.KeepAlive));

        public bool SetBit(uint address, byte value)
        {
            _message_handler($"LuaConnector is setting a bit. [${address:X6} = #${value:X2}]");
            LuaBlock block = new LuaBlock(NextID, LuaBlock.CommandType.SetBits)
            {
                Address = address,
                Value = value
            };
            return WriteBlock(block);
        }

        public bool UnsetBit(uint address, byte value)
        {
            _message_handler($"LuaConnector is clearing a bit. [${address:X6} = #${value:X2}]");
            LuaBlock block = new LuaBlock(NextID, LuaBlock.CommandType.UnsetBits)
            {
                Address = address,
                Value = value
            };
            return WriteBlock(block);
        }

        public bool WriteByte(uint address, byte value)
        {
            _message_handler($"LuaConnector is writing a byte. [${address:X6} = #${value:X2}]");
            LuaBlock block = new LuaBlock(NextID, LuaBlock.CommandType.WriteByte)
            {
                Address = address,
                Value = value
            };
            return WriteBlock(block);
        }

        public bool WriteWord(uint address, ushort value)
        {
            _message_handler($"LuaConnector is writing a word. [${address:X6} = #${value:X4}]");
            LuaBlock block = new LuaBlock(NextID, LuaBlock.CommandType.WriteWord)
            {
                Address = address,
                Value = value
            };
            return WriteBlock(block);
        }

        public byte? ReadByte(uint address)
        {
            uint id = NextID;
            _message_handler($"LuaConnector is reading a byte. [${address:X6}]");
            LuaBlock block = new LuaBlock(id, LuaBlock.CommandType.ReadByte) { Address = address };
            if (!WriteBlock(block)) { return null; }
            _response_events.Put(id, new ManualResetEvent(false));
            // ReSharper disable once PossibleNullReferenceException
            _response_events[id].WaitOne();
            return (byte?)_response_values[id];
        }

        public ushort? ReadWord(uint address)
        {
            uint id = NextID;
            _message_handler($"LuaConnector is reading a word. [${address:X6}]");

            LuaBlock block = new LuaBlock(id, LuaBlock.CommandType.ReadWord) { Address = address };
            if (!WriteBlock(block)) { return null; }
            _response_events.Put(id, new ManualResetEvent(false));
            // ReSharper disable once PossibleNullReferenceException
            _response_events[id].WaitOne();
            return (ushort?)_response_values[id];
        }

        /// <summary>
        /// Sends a message to be displayed.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <returns>True if the text display was successful, otherwise false.</returns>
        /// <remarks>Connector implementations that do not support messaging should return false rather than throwing a NotSupportedException.</remarks>
        public bool SendMessage(string message)
        {
            LuaBlock block = new LuaBlock(NextID, LuaBlock.CommandType.SendMessage) { Message = message ?? string.Empty };
            return WriteBlock(block);
        }

        /// <summary>
        /// Writes a LuaBlock to the socket.
        /// </summary>
        /// <param name="block">The LuaBlock to send.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool WriteBlock([CanBeNull] LuaBlock block)
        {
            if (_stream == null) { return false; }
            string json = JsonConvert.SerializeObject(block);
            if (json == null) { return false; }
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            try
            {
                lock (_stream)
                {
                    _stream.Write(buffer, 0, buffer.Length);
                    _stream.WriteByte(0);
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Reads and enqueues a LuaBlock JSON string.
        /// </summary>
        /// <param name="json">The JSON string describing the LuaBlock to be processed.</param>
        private void ProcessBlock([CanBeNull] string json)
        {
            LuaBlock request = JsonConvert.DeserializeObject<LuaBlock>(json);
            if (request == null) { return; }
            if (request.Type != LuaBlock.CommandType.KeepAlive) {
                _message_handler($"LuaConnector got a block. [{request.ID}]");
            }
            _response_values.Put(request.ID, request.Value);
            if (_response_events.ContainsKey(request.ID)) { _response_events[request.ID]?.Set(); }
        }
    }
}
