using System;
using System.Linq;
using System.Text;
using WorkBench.UOMS;
using System.Threading;
using System.Globalization;
using WorkBench.Interfaces;
using System.Threading.Tasks;
using WorkBench.Communicators;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WorkBench.Interfaces.InstrumentChannel;
using System.Windows.Forms;

namespace WorkBench.TestEquipment.CPC6000
{
    public class CPC6000 : IInstrument
    {
        log4net.ILog logger { get; }

        internal ITextCommunicator Communicator { get; }

        private CPC6000ChannelNumber ActiveChannelNumber { get; set; } 

        public CPC6000(ITextCommunicator communicator)
        {
            logger = log4net.LogManager.GetLogger(typeof(CPC6000));
            logger.Info($"CPC created for {communicator}");
            //IsOpen = false;
            Communicator = communicator;
            ActiveChannelNumber = CPC6000ChannelNumber.Unknown;
        }

        public IInstrumentChannel[] Channels { get; private set; }

        /// <summary>
        /// название типа прибора
        /// </summary>
        public string Name => "CPC6000";

        /// <summary>
        /// Наименование прибора
        /// </summary>
        public string Description => "Калибратор давления";

        public bool IsOpen => Communicator.IsOpen;
        /// <summary>
        /// устанавливает связь с прибором.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> в случае успеха
        /// </returns>
        public bool Open()
        {
            lock (Communicator)
            {
                if (IsOpen) return true;

                //var logger = log4net.LogManager.GetLogger("CPC6000Communication");

                logger.Debug($"CPC6000.Open( {Communicator} ) ");

                bool communicatorOpened = Communicator.Open();

                logger.Debug($"CPC6000.Open( {Communicator} ) communicatorOpened = {communicatorOpened}");

                if (!communicatorOpened) return false;

                var answerStatus = Query("ID?", out string answer, 
                    s =>
                    {
                        var answerparts = s.Split(new char[] { ',' });
                        return answerparts[0].Trim().ToUpper() == "MENSOR" && answerparts[1].Trim().ToUpper() == "600";
                    }
                    );
                if (answerStatus != TextCommunicatorQueryCommandStatus.Success)
                {
                    log4net.LogManager.GetLogger("CPC6000Communication").Debug($"CPC000 didn't answered ");
                    log4net.LogManager.GetLogger("CPC6000Communication").Debug($"communicator close( {Communicator} ) ");
                    Communicator.Close();
                    return false;
                }

                string[] answerParts;

                answerParts = answer.Split(new char[] { ',' }).Select( ap => ap.Trim()).ToArray();

                SerialNo = answerParts[2];
                SwVer = answerParts[3];

                #region Setup Channels

                var CPC6000ChannelA = CPC6000Channel.GetCPC6000Channel(this, CPC6000ChannelNumber.A);

                var CPC6000ChannelB = CPC6000Channel.GetCPC6000Channel(this, CPC6000ChannelNumber.B);

                Channels = (new IInstrumentChannel[] { CPC6000ChannelA, CPC6000ChannelB }).Where(c => c!= null).ToArray();

                if (!Channels.Any())
                {
                    Communicator.Close();
                    return false;
                }

                #endregion

                return true;
            }
        }
        public bool Close()
        {
            lock (Communicator)
            {
                return Communicator.Close();
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ( {2} )", Description, Name, Communicator);
        }

        internal void SetActiveChannel(CPC6000ChannelNumber cPC6000ChannelNumber)
        {
            if (ActiveChannelNumber != cPC6000ChannelNumber)
            {
                switch (cPC6000ChannelNumber)
                {
                    case CPC6000ChannelNumber.A:
                        Communicator.SendLine("Chan A");
                        break;
                    case CPC6000ChannelNumber.B:
                        Communicator.SendLine("Chan B");
                        break;
                    case CPC6000ChannelNumber.Baro:
                        break;

                }
                ActiveChannelNumber = cPC6000ChannelNumber;
            }
        }

        #region CPC6000 hardware interface commands
        //####################################################
        public string SerialNo { get; private set; }
        public string SwVer { get; private set; }
        public int LastErrNo { get; private set; }
        public string LastErrDesc { get; private set; }
        private void GetLastError()
        {
            var answerStatus = Query("Errorno?", out string answer);
            string[] errorparts = answer.Split(new char[] { '-' });
            if (errorparts.Length == 2)
            {
                if (int.TryParse(errorparts[0].TrimStart(new char[] { 'E' }), out int _lastErrNo))
                {
                    LastErrNo = _lastErrNo;
                    LastErrDesc = errorparts[1];
                }
            }
            log4net.LogManager.GetLogger("CPC6000Communication").Debug($"{LastErrNo} === {LastErrDesc}");
            return;
        }
        private void ResetError()
        {
            LastErrNo = 0;
            LastErrDesc = "";
        }

        [Obsolete("НЕ РАБОТАЕТ! НА ЭКРАНЕ ОТОБРАЖАЮТСЯ СООБЩЕНИЯ ОБ ОШИБКЕ", true)]
        internal void KeyLock(bool state)
        {
            switch (state)
            {
                case true:
                    Communicator.SendLine("Keylock Yes");
                    break;
                case false:
                    Communicator.SendLine("Keylock No");
                    break;
            }
        }

        public TextCommunicatorQueryCommandStatus Query(string cmd, out string answer)
        {
            return Query(cmd, out answer, null);
        }

        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer, Func<string, bool> validationRule)
        {
            ResetError();
            Func<string, bool> formatValidationRule = (s) => (s.ToUpper().StartsWith(" ") | s.ToUpper().StartsWith("E"));
            Func<string, bool> finalValidationRule;
            if (validationRule != null)
            {
                finalValidationRule = (s) =>
                {
                    bool b1 = formatValidationRule(s);
                    s = s.TrimStart(new char[] { ' ', 'E' });
                    bool b2 = validationRule(s);
                    return b1 & b2;
                };
            }
            else
            {
                finalValidationRule = formatValidationRule;
            }

            var answerStatus = Communicator.QueryCommand(cmd, out answer, finalValidationRule);

            if (answer.StartsWith("E"))
            {
                GetLastError();

                answer = answer.TrimStart(new char[] { 'E' });
            }

            answer = answer.Trim();

            return answerStatus;
        }

        //####################################################
        #endregion

    }
}