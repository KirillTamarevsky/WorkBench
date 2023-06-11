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

        private CPC6000Channel ActiveChannel { get; set; } 

        public CPC6000(ITextCommunicator communicator)
        {
            logger = log4net.LogManager.GetLogger(typeof(CPC6000));
            logger.Info($"CPC created for {communicator}");
            //IsOpen = false;
            Communicator = communicator;
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

                bool cpcanswered = false;

                var logger = log4net.LogManager.GetLogger("CPC6000Communication");

                logger.Debug($"CPC6000.Open( {Communicator} ) ");

                bool communicatorOpened = Communicator.Open();

                logger.Debug($"CPC6000.Open( {Communicator} ) communicatorOpened = {communicatorOpened}");

                if (!communicatorOpened) return false;

                string answer = Query("ID?");

                string[] answerParts;

                if (!string.IsNullOrEmpty(answer))
                {
                    answerParts = answer.Split(new char[] { ',' });
                    for (int i = 0; i < answerParts.Length; i++)
                    {
                        answerParts[i] = answerParts[i].Trim();
                    }

                    if (answerParts[0] == "MENSOR" && answerParts[1] == "600")
                    {
                        cpcanswered = true;
                        SerialNo = answerParts[2];
                        SwVer = answerParts[3];

                        #region Setup Channels

                        var CPC6000ChannelA = new CPC6000Channel_A(this);

                        var CPC6000ChannelB = new CPC6000Channel_B(this);

                        Channels = new IInstrumentChannel[] { CPC6000ChannelA, CPC6000ChannelB };

                        #endregion

                    }
                }
                if (!cpcanswered)
                {
                    log4net.LogManager.GetLogger("CPC6000Communication").Debug($"CPC000 didn't answered ");
                    log4net.LogManager.GetLogger("CPC6000Communication").Debug($"communicator close( {Communicator} ) ");
                    Communicator.Close();
                }

                return cpcanswered;
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

        internal void SetActiveChannel(CPC6000Channel cPC6000Channel)
        {
            if (cPC6000Channel == null) throw new Exception("null channel!");
            if (cPC6000Channel.parentCPC6000 != this) throw new Exception("this channel is not mine");
            if (ActiveChannel != cPC6000Channel)
            {
                SetActiveChannel(cPC6000Channel.ChannelNumber);
            }
            ActiveChannel = cPC6000Channel;
        }
        internal void SetActiveChannel(CPC6000ChannelNumber cPC6000ChannelNumber)
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
        }

        #region CPC6000 hardware interface commands
        //####################################################
        public string SerialNo { get; private set; }
        public string SwVer { get; private set; }
        public int LastErrNo { get; private set; }
        public string LastErrDesc { get; private set; }
        private void GetLastError()
        {
            string answer = Communicator.QueryCommand("Errorno?");
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

        public string Query(string cmd)
        {
            ResetError();

            string answer = Communicator.QueryCommand(cmd);

            if (answer.StartsWith("E"))
            {
                GetLastError();

                answer = answer.TrimStart(new char[] { 'E' });
            }

            answer = answer.Trim();

            return answer;
        }

        //####################################################
        #endregion

    }
}