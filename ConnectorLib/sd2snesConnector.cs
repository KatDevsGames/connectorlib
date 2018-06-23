using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using WebSocketSharp;
using System.IO;
using System.Linq;
using System.Threading;
using ConnectorLib.usb2snes;
using System.Web.Script.Serialization;

// ReSharper disable InconsistentNaming

namespace ConnectorLib
{
    public class sd2snesConnector : ISNESConnector
    {
        private const uint WORK_BANK = 0xFA0000;
        private const uint BASE_ADDRESS = 0x2C00;

        public struct WriteDescriptor
        {
            public uint address;
            public uint translatedAddress;
            public byte[] data;

            public WriteDescriptor(uint _address, uint _translatedAddress, byte[] _data)
            {
                address = _address;
                translatedAddress = _translatedAddress;
                data = new byte[_data.Length];
                _data.CopyTo(data, 0);
            }
        }

        private Object mTransmitLock = new Object();

        [CanBeNull] private readonly Action<string> mMessageProcessor;

        [NotNull] private readonly JavaScriptSerializer mSerializer = new JavaScriptSerializer();

        [CanBeNull] private WebSocket mSocket;

        private ulong mWrittenMessageIndex = 0;

        [CanBeNull] private System.Timers.Timer mMessageClearTimer;

        //private const uint BASE_ADDRESS = 0;

        [CanBeNull]
        private Action<MessageEventArgs> mPendingMessageHandler;

        public void Dispose()
        {
            if (mSocket != null)
            {
                mSocket.Close();
                mSocket = null;
            }
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
                return mSocket != null && mSocket.IsAlive;
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
                Disconnect();

                // ReSharper disable once UseObjectOrCollectionInitializer
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
                            Operands = new List<string>(new[] { port })
                        }));

                        mSocket.Send(mSerializer.Serialize(new RequestType()
                        {
                            Opcode = OpcodeType.Name.ToString(),
                            Space = "SNES",
                            Operands = new List<string>(new[] { "BitRaces" })
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
            }
        }

        private void SetPendingMessageHandler([CanBeNull] Action<MessageEventArgs> handler) { mPendingMessageHandler = handler; }

        private void Socket_OnMessageReceived([CanBeNull] object sender, [CanBeNull] MessageEventArgs e)
        {
            Action<MessageEventArgs> handler = mPendingMessageHandler;
            mPendingMessageHandler = null;

            handler?.Invoke(e);
        }

        public static bool MapAddressInRange(uint address, uint srcRangeBegin, uint srcRangeEnd, uint dstRangeBegin, out uint mappedAddress)
        {
            if (address >= srcRangeBegin && address <= srcRangeEnd)
            {
                mappedAddress = (address - srcRangeBegin) + dstRangeBegin;
                return true;
            }

            mappedAddress = 0;
            return false;
        }

        public enum TranslationMode
        {
            Read,
            Write
        }

        //  Pre-process the address to map into SD2SNES space
        public static uint TranslateAddress(uint address, TranslationMode mode)
        {
            uint translatedAddress;

            //  WRAM
            if (mode == TranslationMode.Read && MapAddressInRange(address, 0x7E0000u, 0x7FFFFFu, 0xF50000u, out translatedAddress))
                return translatedAddress;

            //  ROM Segments
            for (uint i = 0u; i < 0x3Fu; i++)
            {
                if (MapAddressInRange(address, (i * 0x010000u) + 0x8000u, (i * 0x010000u) + 0xFFFFu, (i * 0x8000u), out translatedAddress))
                    return translatedAddress;
                if (MapAddressInRange(address, (i * 0x010000u) + 0x808000u, (i * 0x010000u) + 0x80FFFFu, (i * 0x8000u), out translatedAddress))
                    return translatedAddress;
            }

            //  SRAM Segments
            for (uint i = 0u; i < 8u; i++)
            {
                if (MapAddressInRange(address, 0x700000u + (i * 0x010000u), 0x707FFF + (i * 0x010000u), 0xE00000 + (i * 0x8000), out translatedAddress))
                    return translatedAddress;
            }

            return address;
        }

        [NotNull] private readonly ManualResetEvent mMemoryReadEvent = new ManualResetEvent(false);

        public bool ReadBytes(uint address, [CanBeNull] byte[] buffer)
        {
            bool bSuccess = false;

            while (!bSuccess)
            {
                ConnectIfNecessary();

                if (!Connected)
                    continue;

                try
                {
                    using (ManualResetEvent readEvent = new ManualResetEvent(false))
                    {
                        lock (mTransmitLock)
                        {
                            RequestType req = new RequestType
                            {
                                Opcode = OpcodeType.GetAddress.ToString(),
                                Space = "SNES",
                                Operands = new List<string>(new[] { TranslateAddress(address, TranslationMode.Read).ToString("X"), buffer.Length.ToString("X") })
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
                                    readEvent.Set();
                                }
                            });

                            mSocket.Send(mSerializer.Serialize(req));
                            readEvent.WaitOne();
                            readEvent.Reset();
                        }
                    }
                }
                catch
                {
                }
            }

            return bSuccess;
        }

        #region Batch Write Support

        ThreadLocal<sd2snesBatchWriteContext> mActiveBatchWriteContext = new ThreadLocal<sd2snesBatchWriteContext>();

        internal void TransmitBatchWrites()
        {
            lock (mTransmitLock)
            {
                try
                {
                    sd2snesBatchWriteContext activeContext = mActiveBatchWriteContext.Value;
                    if (activeContext == null || activeContext.BatchWriteEntries.Count == 0)
                        return;

                    TransmitWriteDescriptors(activeContext.BatchWriteEntries);
                }
                finally
                {
                    mActiveBatchWriteContext.Value = null;
                }
            }
        }

        public IBatchWriteContext OpenBatchWriteContext()
        {
            lock (mTransmitLock)
            {
                Debug.Assert(mActiveBatchWriteContext.Value == null, "Attempting to open a batch write context while a context is already active for the current thread.");
                if (mActiveBatchWriteContext.Value != null)
                    return mActiveBatchWriteContext.Value;

                mActiveBatchWriteContext.Value = new sd2snesBatchWriteContext(this);
                return mActiveBatchWriteContext.Value;
            }
        }
        #endregion

        private void BeginCommand(List<byte> cmd)
        {
            cmd.Clear();

            // PHP
            cmd.Add(0x08);
            // PHB
            cmd.Add(0x8B);
            // SEP #20
            cmd.Add(0xE2);
            cmd.Add(0x20);
            // PHA
            cmd.Add(0x48);
            // XBA
            cmd.Add(0xEB);
            // PHA
            cmd.Add(0x48);
            // LDA #$00
            cmd.Add(0xA9);
            cmd.Add(0x00);
            // PHA
            cmd.Add(0x48);
            // PLB
            cmd.Add(0xAB);
        }

        private static void EndCommand(List<byte> cmd)
        {
            // LDA #$00
            cmd.Add(0xA9);
            cmd.Add(0x00);
            // STA.w $2C00
            cmd.Add(0x8D);
            cmd.Add(0x00);
            cmd.Add(0x2C);
            // PLA
            cmd.Add(0x68);
            // XBA
            cmd.Add(0xEB);
            // PLA
            cmd.Add(0x68);
            // PLB
            cmd.Add(0xAB);
            // PLP
            cmd.Add(0x28);
            // JMP ($FFEA)
            cmd.Add(0x6C);
            cmd.Add(0xEA);
            cmd.Add(0xFF);
        }

        private void AddSystemWriteToCommand(List<byte> cmd, uint address, byte[] data)
        {
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
        }

        private void CommitCommand([NotNull]List<byte> cmd, [CanBeNull] List<WriteDescriptor> shadowWrites = null)
        {
            EndCommand(cmd);

            RequestType req = new RequestType { Opcode = OpcodeType.PutAddress.ToString(), Space = "CMD", Operands = new List<string>(new string[] { 0x002C00.ToString("X"), cmd.Count.ToString("X"), 0x002C00.ToString("X"), 1.ToString("X") }) };
            // Perform first byte last
            cmd.Add(cmd[0]);
            cmd[0] = 0x0;
            mSocket.Send(mSerializer.Serialize(req));
            mSocket.Send(cmd.ToArray());

            //  Write the data into shadow RAM as well, at the read address, to ensure that dependent reads work
            foreach (var entry in shadowWrites ?? Enumerable.Empty<WriteDescriptor>())
            {
                uint shadowAddress = TranslateAddress(entry.address, TranslationMode.Read);
                if (shadowAddress != entry.address)
                {
                    RequestType shadowReq = new RequestType
                    {
                        Opcode = OpcodeType.PutAddress.ToString(),
                        Space = "SNES",
                        Operands = new List<string>(new[] { shadowAddress.ToString("X"), entry.data.Length.ToString("X") })
                    };

                    mSocket.Send(mSerializer.Serialize(shadowReq));
                    mSocket.Send(entry.data);
                }
            }

            cmd.Clear();
            shadowWrites.Clear();
        }

        private void TransmitWriteDescriptors(IEnumerable<WriteDescriptor> descriptors)
        {
            try
            {
                ConnectIfNecessary();

                while (!Connected)
                {
                    //  Infinite connect loop for now, fuck it
                    // DON'T FUCKING CALL THIS FROM GUI THREAD - KKAT
                }

                // ReSharper disable once UseObjectOrCollectionInitializer
                List<byte> cmd = new List<byte>();
                List<WriteDescriptor> shadowWrites = new List<WriteDescriptor>();

                int runningBytes = 0;

                foreach (WriteDescriptor descriptor in descriptors)
                {
                    bool bTranslated = descriptor.address != descriptor.translatedAddress;
                    bool bCommandInProgress = cmd.Count > 0;

                    if (!bTranslated) //IMPORTANT - you can't do more than about 24-32 bytes in a single operation using this
                    {
                        if (descriptor.data.Length < 16)
                        {
                            if (!bCommandInProgress) { BeginCommand(cmd); }

                            AddSystemWriteToCommand(cmd, descriptor.address, descriptor.data);
                            shadowWrites.Add(descriptor);
                            runningBytes += descriptor.data.Length;
                            if (runningBytes > 16)
                            {
                                CommitCommand(cmd, shadowWrites);
                                runningBytes = 0;
                            }
                        }
                        else
                        {
                            if (bCommandInProgress) { CommitCommand(cmd, shadowWrites); }
                            DMAToWRAM(descriptor.address, descriptor.data);
                        }
                    }
                    else
                    {
                        //  Commit any unfinished command
                        if (bCommandInProgress) { CommitCommand(cmd, shadowWrites); }

                        //  Translated (sd2snes) writes happen natively on the cart
                        RequestType request = new RequestType
                        {
                            Opcode = OpcodeType.PutAddress.ToString(),
                            Space = "SNES",
                            Operands = new List<string>(new[] { descriptor.translatedAddress.ToString("X"), descriptor.data.Length.ToString("X") })
                        };

                        mSocket.Send(mSerializer.Serialize(request));
                        mSocket.Send(descriptor.data);
                    }
                }

                //  Commit any unfinished command
                if (cmd.Count > 0) { CommitCommand(cmd, shadowWrites); }
            }
            catch
            {
            }
            finally
            {
                mActiveBatchWriteContext.Value = null;
            }
        }

        private bool DMAToWRAM(uint address, [CanBeNull] byte[] data)
        {
            if (data == null) { return false; }
            if (data.Length > 0x10000) { throw new ArgumentException("The maximum DMA request size is 65536 bytes.", nameof(data)); }

            try
            {
                ConnectIfNecessary();

                if (!Connected) { return false; }

                //  Translated (sd2snes) writes happen natively on the cart
                RequestType request = new RequestType
                {
                    Opcode = OpcodeType.PutAddress.ToString(),
                    Space = "SNES",
                    Operands = new List<string>(new[] { WORK_BANK.ToString("X"), data.Length.ToString("X") })
                };

                mSocket.Send(mSerializer.Serialize(request));
                mSocket.Send(data);

                List<byte> cmd = new List<byte>();
                BeginCommand(cmd);
                // ======== THE DMA ========
                for (int i = 0; i <= 6; i++)
                {
                    cmd.AddRange(SNESAssembler.Instructions.LDA_as((ushort)(0x4300 + i)));
                    cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x2DF0 + i)));
                }
                //set DMA parameters
                cmd.AddRange(SNESAssembler.Instructions.STZ_as((ushort)(0x4300)));

                //set bus B to WRAM port
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)0x80));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x4301)));

                //set WRAM port destination to the target address
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)(address & 0xFF)));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x2181)));
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)((address >> 8) & 0xFF)));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x2182)));
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)(address >> 16)));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x2183)));

                //set bus A source address to the shadow buffer
                cmd.AddRange(SNESAssembler.Instructions.STZ_as((ushort)(0x4302)));
                cmd.AddRange(SNESAssembler.Instructions.STZ_as((ushort)(0x4303)));
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)0xFA));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x4304)));

                //set the transfer size
                ushort dataSize = (ushort)((data.Length >= 0x10000) ? 0 : ((ushort)data.Length));
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)(dataSize & 0xFF)));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x4305)));
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)(dataSize >> 8)));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x4306)));
                cmd.AddRange(SNESAssembler.Instructions.STZ_as((ushort)(0x4307)));

                //begin transfer
                cmd.AddRange(SNESAssembler.Instructions.LDA_c((byte)0x01));
                cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x420B)));
                for (int i = 0; i <= 6; i++)
                {
                    cmd.AddRange(SNESAssembler.Instructions.LDA_as((ushort)(0x2DF0 + i)));
                    cmd.AddRange(SNESAssembler.Instructions.STA_as((ushort)(0x4300 + i)));
                }
                // ======== END DMA ========
                CommitCommand(cmd, new List<WriteDescriptor>(new[] { new WriteDescriptor(address, TranslateAddress(address, TranslationMode.Read), data) })); //yes, TranslationMode.Read - kkat

                return true;
            }
            catch { return false; }
            finally { mActiveBatchWriteContext.Value = null; }
        }

        public bool WriteBytes(uint address, [NotNull] byte[] data)
        {
            lock (mTransmitLock)
            {
                if (mActiveBatchWriteContext.Value != null && data != null && data.Length > 0)
                {
                    mActiveBatchWriteContext.Value.BatchWriteEntries.Add(new WriteDescriptor(address, TranslateAddress(address, TranslationMode.Write), data));
                    return true;
                }
                if (data.Length < 16)
                {
                    WriteDescriptor[] descriptors = new WriteDescriptor[]
                    {
                        new WriteDescriptor(address, TranslateAddress(address, TranslationMode.Write), data)
                    };
                    TransmitWriteDescriptors(descriptors);
                }
                else
                {
                    DMAToWRAM(address, data);
                }
            }
            return true;
        }

        public byte? ReadByte(uint address)
        {
            Output($"sd2snesConnector is reading a byte: [${address:X6}]");
            byte[] buffer = new byte[1];
            if (ReadBytes(address, buffer))
            {
                return buffer[0];
            }

            Output($"sd2snesConnector failed to read a byte: [${address:X6}]");
            return null;
        }

        public ushort? ReadWord(uint address)
        {
            Output($"sd2snesConnector is reading a word: [${address:X6}]");
            byte[] buffer = new byte[2];
            if (ReadBytes(address, buffer))
            {
                return (ushort)(((ushort)buffer[1] << 8) | (ushort)buffer[0]);
            }

            Output($"sd2snesConnector failed to read a word: [${address:X6}]");
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
            Output($"sd2snesConnector is setting a bit: [${address:X6} = #${value:X2}]");

            bool bSuccess = false;

            byte[] buffer = new byte[1];
            if (ReadBytes(address, buffer))
            {
                buffer[0] = (byte)((buffer[0] | value) & 0xFF);
                bSuccess = WriteBytes(address, buffer);
            }

            return bSuccess; //is bSuccess ever not true? - should this be inside a precompiler directive?
        }

        public bool UnsetBit(uint address, byte value)
        {
            Output($"sd2snesConnector is clearing a bit: [${address:X6} = #${value:X2}]");

            bool bSuccess = false;

            byte[] buffer = new byte[1];
            if (ReadBytes(address, buffer))
            {
                buffer[0] = (byte)((buffer[0] & (byte)(~value & 0xFF)) & 0xFF);
                bSuccess = WriteBytes(address, buffer);
            }

            return bSuccess; //is bSuccess ever not true? - should this be inside a precompiler directive?
        }

        [NotNull]
        private readonly object writelock = new object();
        public bool WriteByte(uint address, byte value)
        {
            Output($"sd2snesConnector is writing a byte: [${address:X6} = #${value:X2}]");
            lock (writelock)
            {
                Thread.Sleep(25);
                byte[] buffer = { value };

                if (WriteBytes(address, buffer))
                    return true;

                Output($"sd2snesConnector failed to write a byte: [${address:X6} = #${value:X2}]");
                return false;
            }
        }

        public bool WriteWord(uint address, ushort value)
        {
            Output($"sd2snesConnector is writing a word: [${address:X6} = #${value:X4}]");
            lock (writelock)
            {
                byte[] buffer = { (byte)(value % 0x100), (byte)(value / 0x100) };

                if (WriteBytes(address, buffer))
                    return true;

                Output($"sd2snesConnector failed to write a word: [${address:X6} = #${value:X4}]");
                return false;
            }
        }
    }
}