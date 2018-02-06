#define USE_WEBSOCKET
#define USE_SAFE_BIT_OPERATIONS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using WebSocketSharp;
using System.IO;
using System.Threading;
using ConnectorLib.usb2snes;
#if USE_WEBSOCKET
using System.Web.Script.Serialization;
#endif

// ReSharper disable InconsistentNaming

namespace ConnectorLib
{
    public class sd2snesConnector : ISNESConnector
    {
        [CanBeNull] private readonly Action<string> mMessageProcessor;

#if USE_WEBSOCKET
        [NotNull] private readonly JavaScriptSerializer mSerializer = new JavaScriptSerializer();
#else
        [CanBeNull] private usb2snes.core mSNES;

        private const int READ_BUFFER_SIZE = 512;

        [NotNull] private readonly byte[] mReadBuffer = new byte[READ_BUFFER_SIZE];
#endif

        [CanBeNull] private WebSocket mSocket;

        private ulong mWrittenMessageIndex = 0;

        [CanBeNull] private System.Timers.Timer mMessageClearTimer;

        //private const uint BASE_ADDRESS = 0;

        [CanBeNull]
        private Action<MessageEventArgs> mPendingMessageHandler;

        public void Dispose()
        {
#if USE_WEBSOCKET
            if (mSocket != null)
            {
                mSocket.Close();
                mSocket = null;
            }
#else
            if (mSNES != null)
            {
                mSNES.Disconnect();
                mSNES = null;
            }
#endif
        }

        public sd2snesConnector([CanBeNull] Action<string> messageHandler)
        {
            mMessageProcessor = messageHandler;
            ConnectIfNecessary();
        }

        private void Output([NotNull] string msg, [NotNull] params object[] tokens)
        {
#if DEBUG
            Debug.WriteLine(msg, tokens);
#endif

            mMessageProcessor?.Invoke(string.Format(msg, tokens));
        }

        public bool Connected
        {
            get
            {
#if USE_WEBSOCKET
                return mSocket != null && mSocket.IsAlive;
#else
                return mSNES != null && mSNES.Connected();
#endif
            }
        }

        private void Disconnect()
        {
            mSocket?.Close();
            mSocket = null;
        }

        private void ConnectIfNecessary()
        {
            if (!Connected)
            {
#if USE_WEBSOCKET
                Disconnect();

                mSocket = new WebSocket("ws://localhost:8080");
                mSocket.Log.Output = (data, message) =>
                {
                    if (!string.IsNullOrWhiteSpace(data.Message))
                        Output(data.Message);
                };

                mSocket.OnMessage += Socket_OnMessageReceived;
                mSocket.Connect();

                if (mSocket.IsAlive)
                {
                    Output("Connected to WebSocket");

                    string port = null;

                    SetPendingMessageHandler(e =>
                    {
                        try
                        {
                            Dictionary<string, object> data = mSerializer.DeserializeObject(e.Data) as Dictionary<string, object>;
                            if (data != null)
                            {
                                object values;
                                if (data.TryGetValue("Results", out values))
                                {
                                    IEnumerable<object> ports = values as IEnumerable<object> ?? new object[0];
                                    foreach (object option in ports)
                                    {
                                        port = option as string;
                                        if (!string.IsNullOrWhiteSpace(port))
                                            break;
                                    }
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            mMemoryReadEvent.Set();
                        }
                    });

                    mSocket.Send(mSerializer.Serialize(new RequestType()
                    {
                        Opcode = OpcodeType.DeviceList.ToString(),
                        Space = "SNES"
                    }));

                    mMemoryReadEvent.WaitOne();
                    mMemoryReadEvent.Reset();

                    if (!string.IsNullOrWhiteSpace(port))
                    {
                        Output("Connecting to SD2SNES via {0}", port);

                        mSocket.Send(mSerializer.Serialize(new RequestType()
                        {
                            Opcode = OpcodeType.Attach.ToString(),
                            Space = "SNES",
                            Operands = new List<string>(new [] {port})
                        }));

                        mSocket.Send(mSerializer.Serialize(new RequestType()
                        {
                            Opcode = OpcodeType.Name.ToString(),
                            Space = "SNES",
                            Operands = new List<string>(new [] {"BitRaces"})
                        }));
                    }
                    else
                    {
                        Output("ERROR: No SD2SNES was detected by the system, or the USB2SNES tray application is not running");
                    }
                }
                else
                {
                    Output("ERROR: Failed to connect to web socket; the USB2SNES tray application may not be running");
                }
#else
                var ports = usb2snes.core.GetDeviceList();
                if (ports == null || ports.Count <= 0)
                {
                    Output("No SD2SNES devices were found on known COM ports");
                    return;
                }

                var port = ports[0];
                if (port != null)
                {
                    usb2snes.core core = new core();
                    core.Connect(port.Name);

                    if (core.Connected())
                    {
                        Output("SD2SNES connected on port {0}", port.Name);
                        mSNES = core;
                    }
                    else
                    {
                        Output("SD2SNES failed to connect on port {0}", port.Name);
                    }
                }
#endif
            }
        }

        private void SetPendingMessageHandler([CanBeNull] Action<MessageEventArgs> handler) { mPendingMessageHandler = handler; }

        private void Socket_OnMessageReceived([CanBeNull] object sender, [CanBeNull] MessageEventArgs e)
        {
            Action<MessageEventArgs> handler = mPendingMessageHandler;
            mPendingMessageHandler = null;

            handler?.Invoke(e);
        }

        //  Pre-process the address to map into SD2SNES space
        private uint TranslateAddress(uint address)
        {
            //  WRAM
            if (address >= 0x7E0000 && address <= 0x7FFFFF)
                return (address - 0x7E0000) + 0xF50000;

            return address;
        }

        [NotNull] private readonly ManualResetEvent mMemoryReadEvent = new ManualResetEvent(false);

        private bool ReadBytes(uint address, [CanBeNull] byte[] buffer)
        {
            bool bSuccess = false;

            while (!bSuccess)
            {
                ConnectIfNecessary();

                if (!Connected)
                    continue;

#if USE_WEBSOCKET
                try
                {
                    //uint formattedAddress = TranslateAddress(address);

                    RequestType req = new RequestType
                    {
                        Opcode = OpcodeType.GetAddress.ToString(),
                        Space = "SNES",
                        Operands = new List<string>(new [] {TranslateAddress(address).ToString("X"), buffer.Length.ToString("X")})
                    };

                    SetPendingMessageHandler(e =>
                    {
                        try
                        {
                            if (e.IsBinary && e.RawData != null)
                            {
                                for (int i = 0; i < buffer.Length; ++i)
                                {
                                    buffer[i] = e.RawData[Math.Min(i, e.RawData.Length - 1)];
                                }

                                bSuccess = true;
                            }
                        }
                        catch { }
                        finally
                        {
                            mMemoryReadEvent.Set();
                        }
                    });

                    mSocket.Send(mSerializer.Serialize(req));
                    mMemoryReadEvent.WaitOne();
                    mMemoryReadEvent.Reset();
                }
                catch { }
#else
            if (mSNES == null)
                return false;

            try
            {
                Array.Clear(mReadBuffer, 0, READ_BUFFER_SIZE);

                int resultLength = (int)mSNES.SendCommand(usbint_server_opcode_e.GET, usbint_server_space_e.SNES, usbint_server_flags_e.NONE, TranslateAddress(address), (uint)buffer.Length);
                mSNES.GetData(buffer, 0, resultLength);

                Output("Successfully read bytes");
                return true;
            }
            catch (Exception e)
            {
                Output("ReadBytes threw an exception: {0}", e);
            }
#endif
            }
            return bSuccess;
        }

        private bool WriteBytes(uint address, [CanBeNull] byte[] data)
        {
#if USE_WEBSOCKET
            bool bSuccess = false;

            while (!bSuccess)
            {
                ConnectIfNecessary();

                if (!Connected)
                    continue;

                try
                {
                    //  To write memory and have it "stick" on the SNES, we need to actually build a little function that will refresh the memory properly
                    //  This assumes we are writing exclusively to WRAM

                    // setup new command
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    List<byte> cmd = new List<byte>();

                    // PHP
                    cmd.Add(0x08);
                    // SEP #20
                    cmd.Add(0xE2);
                    cmd.Add(0x20);
                    // PHA
                    cmd.Add(0x48);
                    // XBA
                    cmd.Add(0xEB);
                    // PHA
                    cmd.Add(0x48);

                    for (uint i = 0; i < data.Length; ++i)
                    {
                        uint elementAddress = address + i;

                        // LDA.l #data[index]
                        cmd.Add(0xA9);
                        cmd.Add(data[i]);
                        // STA.l $address+i
                        cmd.Add(0x8F);
                        cmd.Add(Convert.ToByte((elementAddress >> 0) & 0xFF));
                        cmd.Add(Convert.ToByte((elementAddress >> 8) & 0xFF));
                        cmd.Add(Convert.ToByte((elementAddress >> 16) & 0xFF));
                    }

                    // LDA #$00
                    cmd.Add(0xA9);
                    cmd.Add(0x00);
                    // STA.l $002C00
                    cmd.Add(0x8F);
                    cmd.Add(0x00);
                    cmd.Add(0x2C);
                    cmd.Add(0x00);
                    // PLA
                    cmd.Add(0x68);
                    // XBA
                    cmd.Add(0xEB);
                    // PLA
                    cmd.Add(0x68);
                    // PLP
                    cmd.Add(0x28);
                    // JMP ($FFEA)
                    cmd.Add(0x6C);
                    cmd.Add(0xEA);
                    cmd.Add(0xFF);

                    RequestType req = new RequestType { Opcode = OpcodeType.PutAddress.ToString(), Space = "CMD", Operands = new List<string>(new string[] { 0x002C00.ToString("X"), cmd.Count.ToString("X"), 0x002C00.ToString("X"), 1.ToString("X") }) };
                    // Perform first byte last
                    cmd.Add(cmd[0]);
                    cmd[0] = 0x0;
                    mSocket.Send(mSerializer.Serialize(req));
                    mSocket.Send(cmd.ToArray());

                    RequestType shadowReq = new RequestType
                    {
                        Opcode = OpcodeType.PutAddress.ToString(),
                        Space = "SNES",
                        Operands = new List<string>(new[] { TranslateAddress(address).ToString("X"), data.Length.ToString("X") })
                    };

                    mSocket.Send(mSerializer.Serialize(shadowReq));
                    mSocket.Send(data);

                    bSuccess = true;
                }
                catch { }
            }
#else
            ConnectIfNecessary();

            if (!Connected)
                return false;

            try
            {
                //  To write memory and have it "stick" on the SNES, we need to actually build a little function that will refresh the memory properly
                //  This assumes we are writing exclusively to WRAM

                // setup new command
                List<Byte> cmd = new List<Byte>();

                // PHP
                cmd.Add(0x08);
                // SEP #20
                cmd.Add(0xE2); cmd.Add(0x20);
                // PHA
                cmd.Add(0x48);
                // XBA
                cmd.Add(0xEB);
                // PHA
                cmd.Add(0x48);

                for (uint i = 0; i < data.Length; ++i)
                {
                    uint elementAddress = address + i;

                    // LDA.l #data[index]
                    cmd.Add(0xA9); cmd.Add(data[i]);
                    // STA.l $address+i
                    cmd.Add(0x8F); cmd.Add(Convert.ToByte((elementAddress >> 0) & 0xFF)); cmd.Add(Convert.ToByte((elementAddress >> 8) & 0xFF)); cmd.Add(Convert.ToByte((elementAddress >> 16) & 0xFF));
                }

                // LDA #$00
                cmd.Add(0xA9); cmd.Add(0x00);
                // STA.l $002C00
                cmd.Add(0x8F); cmd.Add(0x00); cmd.Add(0x2C); cmd.Add(0x00);
                // PLA
                cmd.Add(0x68);
                // XBA
                cmd.Add(0xEB);
                // PLA
                cmd.Add(0x68);
                // PLP
                cmd.Add(0x28);
                // JMP ($FFEA)
                cmd.Add(0x6C); cmd.Add(0xEA); cmd.Add(0xFF);

                mSNES.SendCommand(usbint_server_opcode_e.VPUT, usbint_server_space_e.CMD, usbint_server_flags_e.NONE, new Tuple<int, int>(0x002C00, cmd.Count), new Tuple<int, int>(0x002C00, 1));

                cmd.Add(cmd[0]); cmd[0] = 0x0;

                byte[] cmdFormatted = new byte[cmd.Count];
                for (int i = 0; i < cmdFormatted.Length; ++i)
                    cmdFormatted[i] = cmd[i];

                mSNES.SendData(cmdFormatted, cmdFormatted.Length);

                return true;
            }
            catch
            {
            }

/*
            if (mSNES == null)
                return false;

            try
            {
                mSNES.SendCommand(usbint_server_opcode_e.PUT, usbint_server_space_e.SNES, usbint_server_flags_e.NONE, FormatAddress(address), (uint)data.Length);
                mSNES.SendData(data, data.Length);

                Output("Successfully wrote bytes");
                return true;
            }
            catch (Exception e)
            {
                Output("ReadBytes threw an exception: {0}", e);
            }
*/
#endif

            return bSuccess; //is bSuccess ever not true? - should this be inside a precompiler directive?
        }

        public byte? ReadByte(uint address)
        {
            byte[] buffer = new byte[1];
            if (ReadBytes(address, buffer))
            {
                return buffer[0];
            }

            Output("Failed to read byte at address {0:x}", address);
            return null;
        }

        public ushort? ReadWord(uint address)
        {
            byte[] buffer = new byte[2];
            if (ReadBytes(address, buffer))
            {
                return (ushort) (((ushort) buffer[1] << 8) | (ushort) buffer[0]);
            }

            Output("Failed to read word at address {0:x}", address);
            return null;
        }

        public bool SendMessage(string message)
        {
            string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BitRaces");
            string outputPath = Path.Combine(outputFolder, "sd2snes_message_log.txt");

            if (!string.IsNullOrWhiteSpace(message))
            {
                ulong messageIdx = ++mWrittenMessageIndex;
                try
                {
                    Directory.CreateDirectory(outputFolder);

                    using (TextWriter writer = new StreamWriter(outputPath, messageIdx != 1))
                    {
                        if (messageIdx != 1)
                            writer.Write('\n');

                        writer.Write(message);

                        //using (SoundPlayer player = new SoundPlayer(Path.Combine(outputFolder, "ding.wav")))
                        //using (SoundPlayer player = new SoundPlayer(Path.Combine(outputFolder, "Utopia Question.wav")))
                        //{
                        //    player.Play();
                        //}

                        return true;
                    }
                }
                catch { }
                finally
                {
                    if (mMessageClearTimer != null)
                    {
                        mMessageClearTimer.Dispose();
                        mMessageClearTimer = null;
                    }

                    mMessageClearTimer = new System.Timers.Timer
                    {
                        Interval = 5000,
                        AutoReset = false
                    };

                    mMessageClearTimer.Elapsed += (sender, args) =>
                    {
                        if (mWrittenMessageIndex == messageIdx)
                        {
                            try
                            {
                                using (TextWriter writer = new StreamWriter(outputPath, true))
                                {
                                    writer.WriteLine("");
                                    writer.WriteLine("");
                                }
                            }
                            catch { }
                        }
                    };

                    mMessageClearTimer.Start();
                }
            }

            return false;
        }

        //private void MMessageClearTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) { throw new NotImplementedException(); }

        public bool SetBit(uint address, byte value)
        {
#if USE_WEBSOCKET
            bool bSuccess = false;

            while (!bSuccess)
            {
                ConnectIfNecessary();

                if (!Connected)
                    continue;

#if USE_SAFE_BIT_OPERATIONS
                byte[] buffer = new byte[1];
                if (ReadBytes(address, buffer))
                {
                    buffer[0] = (byte)((buffer[0] | value) & 0xFF);
                    bSuccess = WriteBytes(address, buffer);
                }
#else

                try
                {
                    //  To write memory and have it "stick" on the SNES, we need to actually build a little function that will refresh the memory properly
                    //  This assumes we are writing exclusively to WRAM

                    // setup new command
                    List<byte> cmd = new List<byte>();

                    // PHP
                    cmd.Add(0x08);
                    // SEP #20
                    cmd.Add(0xE2);
                    cmd.Add(0x20);
                    // PHA
                    cmd.Add(0x48);
                    // XBA
                    cmd.Add(0xEB);
                    // PHA
                    cmd.Add(0x48);

                    // LDA.l $address
                    cmd.Add(0xAF);
                    cmd.Add(Convert.ToByte((address >> 0) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 8) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 16) & 0xFF));

                    // ORA #value
                    cmd.Add(0x09);
                    cmd.Add(value);

                    // STA.l $addr
                    cmd.Add(0x8F);
                    cmd.Add(Convert.ToByte((address >> 0) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 8) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 16) & 0xFF));

                    // LDA #$00
                    cmd.Add(0xA9);
                    cmd.Add(0x00);
                    // STA.l $002C00
                    cmd.Add(0x8F);
                    cmd.Add(0x00);
                    cmd.Add(0x2C);
                    cmd.Add(0x00);
                    // PLA
                    cmd.Add(0x68);
                    // XBA
                    cmd.Add(0xEB);
                    // PLA
                    cmd.Add(0x68);
                    // PLP
                    cmd.Add(0x28);
                    // JMP ($FFEA)
                    cmd.Add(0x6C);
                    cmd.Add(0xEA);
                    cmd.Add(0xFF);

                    RequestType req = new RequestType() {Opcode = OpcodeType.PutAddress.ToString(), Space = "CMD", Operands = new List<string>(new string[] {0x002C00.ToString("X"), cmd.Count.ToString("X"), 0x002C00.ToString("X"), 1.ToString("X")})};
                    // Perform first byte last
                    cmd.Add(cmd[0]);
                    cmd[0] = 0x0;
                    mSocket.Send(mSerializer.Serialize(req));
                    mSocket.Send(cmd.ToArray());

                    bSuccess = true;
                }
                catch { }
#endif
            }
#else
            throw new NotImplementedException();
#endif

                return bSuccess; //is bSuccess ever not true? - should this be inside a precompiler directive?
        }

        public bool UnsetBit(uint address, byte value)
        {
#if USE_WEBSOCKET

            bool bSuccess = false;

            while (!bSuccess)
            {
                ConnectIfNecessary();

                if (!Connected)
                    continue;

#if USE_SAFE_BIT_OPERATIONS
                byte[] buffer = new byte[1];
                if (ReadBytes(address, buffer))
                {
                    buffer[0] =(byte)((buffer[0] & (byte)(~value & 0xFF)) & 0xFF);
                    bSuccess = WriteBytes(address, buffer);
                }
#else
                try
                {
                    //  To write memory and have it "stick" on the SNES, we need to actually build a little function that will refresh the memory properly
                    //  This assumes we are writing exclusively to WRAM

                    // setup new command
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    List<byte> cmd = new List<byte>();

                    // PHP
                    cmd.Add(0x08);
                    // SEP #20
                    cmd.Add(0xE2);
                    cmd.Add(0x20);
                    // PHA
                    cmd.Add(0x48);
                    // XBA
                    cmd.Add(0xEB);
                    // PHA
                    cmd.Add(0x48);

                    // LDA.l $address
                    cmd.Add(0xAF);
                    cmd.Add(Convert.ToByte((address >> 0) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 8) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 16) & 0xFF));

                    // AND #value
                    cmd.Add(0x29);
                    cmd.Add((byte) (~value & 0xFF));

                    // STA.l $addr
                    cmd.Add(0x8F);
                    cmd.Add(Convert.ToByte((address >> 0) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 8) & 0xFF));
                    cmd.Add(Convert.ToByte((address >> 16) & 0xFF));

                    // LDA #$00
                    cmd.Add(0xA9);
                    cmd.Add(0x00);
                    // STA.l $002C00
                    cmd.Add(0x8F);
                    cmd.Add(0x00);
                    cmd.Add(0x2C);
                    cmd.Add(0x00);
                    // PLA
                    cmd.Add(0x68);
                    // XBA
                    cmd.Add(0xEB);
                    // PLA
                    cmd.Add(0x68);
                    // PLP
                    cmd.Add(0x28);
                    // JMP ($FFEA)
                    cmd.Add(0x6C);
                    cmd.Add(0xEA);
                    cmd.Add(0xFF);

                    RequestType req = new RequestType { Opcode = OpcodeType.PutAddress.ToString(), Space = "CMD", Operands = new List<string>(new string[] { 0x002C00.ToString("X"), cmd.Count.ToString("X"), 0x002C00.ToString("X"), 1.ToString("X") }) };
                    // Perform first byte last
                    cmd.Add(cmd[0]);
                    cmd[0] = 0x0;
                    mSocket.Send(mSerializer.Serialize(req));
                    mSocket.Send(cmd.ToArray());

                    bSuccess = true;
                }
                catch { }
#endif
            }
#else
            throw new NotImplementedException();
#endif

                return bSuccess; //is bSuccess ever not true? - should this be inside a precompiler directive?
        }

        public bool WriteByte(uint address, byte value)
        {
            byte[] buffer = {value};

            if (WriteBytes(address, buffer))
                return true;

            Output("Failed to write byte at address {0:x}", address);
            return false;
        }

        public bool WriteWord(uint address, ushort value)
        {
            byte[] buffer = {(byte) (value % 0x100), (byte) (value / 0x100)};

            if (WriteBytes(address, buffer))
                return true;

            Output("Failed to write word at address {0:x}", address);
            return false;
        }
    }
}
