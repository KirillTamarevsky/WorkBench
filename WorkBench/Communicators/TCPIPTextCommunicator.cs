using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Primitives;

namespace WorkBench.Communicators
{
    public class TCPIPTextCommunicator : ITextCommunicator, IDisposable
    {
        readonly ILog logger = LogManager.GetLogger("Communication");

        IPAddress _ipaddress;
        int _port;

        Socket _tcpClient;

        readonly List<byte> _receivedBytes = new();

        private string NewLine { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; }

        public int CommunicationRetries { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="baudrate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="lineEndToken">символы конца строки</param>
        public TCPIPTextCommunicator(
            IPAddress ipaddress,
            int port,
            string lineEndToken,
            int timeout = 3,
            int communicationRetries = 3)
        {
            _ipaddress = ipaddress;
            _port = port;

            NewLine = lineEndToken;

            Timeout = TimeSpan.FromSeconds(timeout); // 3 seconds

            CommunicationRetries = communicationRetries;
        }
        public bool Open()
        {
            bool clientOpened = false;

            if (_tcpClient == null)
            {
                try
                {
                    _tcpClient = new(_ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    _tcpClient.ReceiveTimeout = (int)Timeout.TotalMilliseconds; 
                    _tcpClient.SendTimeout = (int)Timeout.TotalMilliseconds;
                    _tcpClient.Connect(_ipaddress, _port);
                return _tcpClient.Connected;
                }
                catch (Exception e)
                {
                    logger.Debug($"Exception while try to open {_ipaddress} | {e}");
                }
            }
            return clientOpened;
        }

        public bool Close()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {

                logger.Info($"Closing SerialEKCommunicator on {_tcpClient.RemoteEndPoint}");

                _tcpClient.Shutdown(SocketShutdown.Both);
                _tcpClient.Disconnect(false);
                _tcpClient.Dispose();
                _tcpClient = null;
            }
            return false;
        }


        public TextCommunicatorReadLineStatus ReadLine(TimeSpan readLineTimeout, out string result)
        {
            result = string.Empty;

            logger.Debug($"{_tcpClient.RemoteEndPoint} Start ReadLine(), Readtimeout = {readLineTimeout}");

            if (!_tcpClient.Connected)
            {
                var errorMessage = $"SerialPort {_tcpClient.RemoteEndPoint} not opened while try to ReadLine";
                logger.Error(errorMessage);
                return TextCommunicatorReadLineStatus.CommunicationChannelClosed;
            }
                    var buffer = new byte[1_024];
            int received;
            try
            {
                    received = _tcpClient.Receive(buffer, SocketFlags.None);
            }
            catch (Exception ex)
            {
                logger.Debug($"ReceiveByteFromSerialPort {_tcpClient.RemoteEndPoint} | Exception = {ex}");
                return TextCommunicatorReadLineStatus.CommunicationError;
            }

            string answer = ASCIIEncoding.ASCII.GetString(buffer, 0, received);
            logger.Info(
                string.Format("{0} ReadLine() answer = \"{1}\" | {2}",
                _tcpClient.RemoteEndPoint,
                answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                BitConverter.ToString(Encoding.ASCII.GetBytes(answer))
                ));

            result = answer.TrimEnd(NewLine.ToCharArray());

            return TextCommunicatorReadLineStatus.Success;

        }

        public TextCommunicatorSendLineStatus SendLine(string cmd)
        {
            logger.Debug($"Start SendLine( {_tcpClient.RemoteEndPoint} )");

            if (!_tcpClient.Connected)
            {
                var errorMessage = $"SerialPort {_tcpClient.RemoteEndPoint} not Connected while try to SendLine";
                logger.Error(errorMessage);
                return TextCommunicatorSendLineStatus.CommunicationChannelClosed;
            }

            try
            {
                var dataToSend = cmd + NewLine;

                logger.Info(
                    string.Format("SendLine( {0} ) dataToSend = {1} | {2}",
                    _tcpClient.RemoteEndPoint,
                    dataToSend.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(dataToSend)))
                    );

                var messageBytes = Encoding.ASCII.GetBytes(dataToSend);
                _tcpClient.Send(messageBytes);

                return TextCommunicatorSendLineStatus.Success;
            }
            catch (TimeoutException)
            {
                logger.Debug($"SendLine({_tcpClient.RemoteEndPoint}) TimeOut");
                return TextCommunicatorSendLineStatus.TimedOut;
            }
            catch (Exception)
            {
                return TextCommunicatorSendLineStatus.CommunicationError;
            }
        }

        public TextCommunicatorQueryCommandStatus QueryCommand(string cmd, out string result, Func<string, bool> validationRule)
        {
            result = string.Empty;

            int trynumber = 0;

            TextCommunicatorQueryCommandStatus status = TextCommunicatorQueryCommandStatus.CommunicationChannelClosed;

            while (trynumber < CommunicationRetries & status != TextCommunicatorQueryCommandStatus.Success)
            {

                switch (SendLine(cmd))
                {
                    case TextCommunicatorSendLineStatus.Success:
                        var readlineStatus = ReadLine(Timeout, out result);
                        switch (readlineStatus)
                        {
                            case TextCommunicatorReadLineStatus.Success:
                                if (validationRule == null || validationRule(result))
                                {
                                    status = TextCommunicatorQueryCommandStatus.Success;
                                }
                                else
                                {
                                    status = TextCommunicatorQueryCommandStatus.CommunicationError;
                                }
                                break;

                            case TextCommunicatorReadLineStatus.TimedOut:
                                status = TextCommunicatorQueryCommandStatus.ReadLineTimedOut;
                                break;

                            case TextCommunicatorReadLineStatus.CommunicationError:
                                status = TextCommunicatorQueryCommandStatus.CommunicationError;
                                break;

                            case TextCommunicatorReadLineStatus.CommunicationChannelClosed:
                                status = TextCommunicatorQueryCommandStatus.CommunicationChannelClosed;
                                break;
                        }
                        break;

                    case TextCommunicatorSendLineStatus.TimedOut:
                        status = TextCommunicatorQueryCommandStatus.SendTimedOut;
                        break;

                    case TextCommunicatorSendLineStatus.CommunicationError:
                        status = TextCommunicatorQueryCommandStatus.CommunicationError;
                        break;

                    case TextCommunicatorSendLineStatus.CommunicationChannelClosed:
                        status = TextCommunicatorQueryCommandStatus.CommunicationChannelClosed;
                        break;
                }
                trynumber++;
            }
            return status;
        }
        public void Dispose()
        {
            Close();
        }

        // the Serial Port detection routine 

        public override string ToString()
        {
            return $"{_ipaddress}";
        }

        public bool IsOpen => _tcpClient != null &&_tcpClient.Connected;
    }
}

