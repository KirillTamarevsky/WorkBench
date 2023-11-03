using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.HartLite
{
    internal class SerialPortWrapper : ISerialPortWrapper
    {
        private readonly SerialPort _serialPort;

        private IntPtr _handle;

        private ConcurrentQueue<FSKReceivedByte> _buffer = new ConcurrentQueue<FSKReceivedByte>();

        public SerialPortWrapper(string comPort, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(comPort, baudrate, parity, dataBits, stopBits);
        }

        public void Open()
        {
            _serialPort.Open();
            Thread.Sleep(100);
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            BeginLoop_ReadSerialPort();
        }

        public void Close()
        {
            EndLoop_ReadSerialPort() ;
            _serialPort.Close();
        }

        public bool IsOpen => _serialPort.IsOpen;
        public int Read(FSKReceivedByte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _buffer.TryDequeue(out buffer[i + offset]);
            }
            //return _serialPort.Read(buffer, offset, count);
            return count;
        }
        public int ReadTimeout
        {
            get => _serialPort.ReadTimeout;
            set => _serialPort.ReadTimeout = value;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.Write(buffer, offset, count);
        }

        public int BytesToRead
        {
            //get { return _serialPort.BytesToRead; }
            get { return _buffer.Count; }
        }

        public bool DtrEnable
        {
            get { return _serialPort.DtrEnable; }
            set { _serialPort.DtrEnable = value; }
        }

        public bool RtsEnable
        {
            get { return _serialPort.RtsEnable; }
            set { _serialPort.RtsEnable = value; }
        }

        public bool CtsHolding
        {
            get { return _serialPort.CtsHolding; }
        }

        public bool CDHolding
        {
            get { return _serialPort.CDHolding; }
        }

        public string PortName
        {
            get { return _serialPort.PortName; }
            set { _serialPort.PortName = value; }
        }

        public event Action<FSKReceivedByte> DataReceived;
        
        CancellationTokenSource CTS_ReadSerial;
        CancellationToken ct_ReadSerial;

        Thread serThread;

        Task ReadSerialTask;
        // this is started when the session starts
        protected void BeginLoop_ReadSerialPort()
        {
            // New token required for each connection
            // because EndLoop() cancels and disposes it each time.
            CTS_ReadSerial?.Dispose();  // should already be disposed
            CTS_ReadSerial = new CancellationTokenSource();
            ct_ReadSerial = CTS_ReadSerial.Token;

            //ReadSerialTask = Task.Run(() => { ReadSerialBytes_AsyncLoop(ct_ReadSerial); });
            serThread = new Thread(() => ReadSerialBytes_AsyncLoopAsync(ct_ReadSerial));
            serThread.Priority = ThreadPriority.Highest;
            serThread.Name = "HART_SerialPortHandle" + serThread.ManagedThreadId;
            serThread.Start();
        }

        protected void EndLoop_ReadSerialPort()
        {
            try
            {
                CTS_ReadSerial?.Cancel();
                //ReadSerialTask?.Wait();
                serThread?.Join();
            }
            catch (Exception e)
            {
                throw e;
                //var typ = Global.ProgramSettings.DbgExceptions;
                if (e is TaskCanceledException)
                {
                    //dbg_EventHandler(typ, $"Task Cancelled: {((TaskCanceledException)e).Task.Id}\n");
                }
                else
                {
                    //dbg_EventHandler(typ, $"Task Exception: {e.GetType().Name}\n");
                }
            }
            finally
            {
                CTS_ReadSerial?.Dispose();
            }
        }
        private async Task ReadSerialBytes_AsyncLoopAsync(CancellationToken ct)
        {
            const int bytesToRead = 512;
            var receiveBuffer = new byte[bytesToRead];
            while ((_serialPort.IsOpen) && (!ct.IsCancellationRequested))
            {
                try
                {
                    if (!_serialPort.RtsEnable)
                    {
                        var datetime_beforeRead = DateTime.Now;
                        _serialPort.BaseStream.ReadTimeout = 20;
                        var receivedCount = await _serialPort.BaseStream?.ReadAsync(receiveBuffer, 0, bytesToRead, ct);
                        var datetime_afterRead = DateTime.Now;

                        if (receivedCount > 0)
                        {
                            var receivedBytesArray = new byte[receivedCount];
                            Array.Copy(receiveBuffer, receivedBytesArray, receivedCount);
                            log4net.LogManager.GetLogger("HART").Debug($"received {1} bytes {BitConverter.ToString(receivedBytesArray)}");
                            for (int i = 0; i < receivedBytesArray.Length; i++)
                            {
                                DataReceived?.Invoke(new FSKReceivedByte(datetime_afterRead, receivedBytesArray[i]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }

            }
        }
        ConstructorInfo SerialDataReceivedEventArgsconstructor = typeof(SerialDataReceivedEventArgs).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(SerialData) },
            null);
        private SerialDataReceivedEventArgs GetSerialDataReceivedEventArgs()
        {
            return (SerialDataReceivedEventArgs)SerialDataReceivedEventArgsconstructor.Invoke(new object[] { SerialData.Chars });
        }

    }
}