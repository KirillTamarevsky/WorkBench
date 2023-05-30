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

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000 : IInstrument
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CPC6000));
        internal ITextCommunicator Communicator { get; }


        public CPC6000(ITextCommunicator communicator)
        {
            logger.Info($"CPC created for {communicator}");
            Communicator = communicator;
        }

        public IInstrumentChannel[] Channels { get; private set; }

        /// <summary>
        /// название типа прибора
        /// </summary>
        public static string Name
        {
            get => "CPC6000";
        }

        /// <summary>
        /// Наименование прибора
        /// </summary>
        public static string Description
        {
            get => "Калибратор давления";
        }
        public bool IsOpen { get; private set; }
        /// <summary>
        /// устанавливает связь с прибором.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> в случае успеха
        /// </returns>
        public bool Open()
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
                    _serialNo = answerParts[2];
                    _swVer = answerParts[3];


                    IsOpen = true; // если пришел ответ, то прибор перешел в режим удаленной работы.
                                   // необходимо перевести в локальный режим по окончании сеанса связи
                                   //KeyLock(true); 

                    SetUOMOnChannel(CPC6000ChannelNumber.A, "kPa");
                    SetUOMOnChannel(CPC6000ChannelNumber.B, "kPa");

                    #region Setup Channels

                    var CPC6000ChannelA = new CPC6000Channel_A(this);

                    var CPC6000ChannelB = new CPC6000Channel_B(this);

                    Channels = new IInstrumentChannel[] { CPC6000ChannelA, CPC6000ChannelB };

                    //this[CPC6000ChannelNumber.Baro] = new CPC6000ChannelBase();
                    #endregion

                }
            }
            if (!cpcanswered)
            {
                log4net.LogManager.GetLogger("CPC6000Communication").Debug($"communicator close( {Communicator} ) ");
                Communicator.Close();
                IsOpen = false;
            }

            return cpcanswered;
        }
        public bool Close()
        {
            IsOpen = Communicator.Close();
            return IsOpen;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ( {2} )", Description, Name, Communicator.ToString());
        }

    }
}