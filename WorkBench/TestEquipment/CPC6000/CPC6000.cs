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
using System.Runtime.Remoting.Channels;
using WorkBench.AbstractClasses.Instrument;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.AbstractClasses.InstrumentCommand;

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000 : AbstractInstrument
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CPC6000));
        public CPC6000(ITextCommunicator communicator):base(communicator)
        {
            logger.Info($"CPC created for {communicator}" );
        }

        private IInstrumentChannel[] _channels;
        public override IInstrumentChannel[] Channels 
        { 
            get => _channels;
        }

        public IInstrumentChannel this[int i] 
        { 
            get
            {
                if (i > 0 | i < 3)
                {
                    return _channels[i];
                }
                return null;
            }
        }

        public CPC6000Channel this[CPC6000ChannelNumber chanNum]
        {
            set
            {
                value.NUM = (int)chanNum;

                value.parent = this;

                _channels[(int)chanNum - 1] = value;
            }
            get
            {
                return (CPC6000Channel)_channels[(int)chanNum - 1];
            }
        }

        /// <summary>
        /// название типа прибора
        /// </summary>
        public override string Name 
        { 
            get => "CPC6000";  
        }

        /// <summary>
        /// Наименование прибора
        /// </summary>
        public override string Description 
        { 
            get => "Калибратор давления";
        }
        private bool isConnected;
        public override bool IsOpen => base.IsOpen && isConnected;
        /// <summary>
        /// устанавливает связь с прибором.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> в случае успеха
        /// </returns>
        public override async Task<bool> Open()
        {
            return await Task.Run(() =>
            {
                if (isConnected) return true;
                
                bool cpcanswered = false;

                var logger = log4net.LogManager.GetLogger("CPC6000Communication");

                logger.Debug($"CPC6000.Open( {Communicator} ) ");

                bool communicatorOpened = Communicator.Open();

                logger.Debug($"CPC6000.Open( {Communicator} ) communicatorOpened = {communicatorOpened}");

                if (communicatorOpened)
                {
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


                            isConnected = true; // если пришел ответ, то прибор перешел в режим удаленной работы.
                                                // необходимо перевести в локальный режим по окончании сеанса связи
                            //KeyLock(true); 

                            SetUOMOnChannel(CPC6000ChannelNumber.A, "kPa");
                            SetUOMOnChannel(CPC6000ChannelNumber.B, "kPa");

                            #region Setup Channels
                                _channels = new CPC6000Channel[2];

                                this[CPC6000ChannelNumber.A] = new CPC6000Channel(this, CPC6000ChannelNumber.A);

                                this[CPC6000ChannelNumber.B] = new CPC6000Channel(this, CPC6000ChannelNumber.B);

                                //this[CPC6000ChannelNumber.Baro] = new CPC6000ChannelBase();
                            #endregion

                            StartCommandQueueProcessor();
                        }
                    }
                    if (!cpcanswered)
                    {
                        log4net.LogManager.GetLogger("CPC6000Communication").Debug($"communicator close( {Communicator} ) ");
                        Communicator.Close();
                        isConnected = false;
                    }

                }
                return cpcanswered;
            });
        }
        public override bool Close()
        {
            StopCommandQueueProcessor();
            if (isConnected)
            {
                //KeyLock(false); // <===== НЕ РАБОТАЕТ! 
                isConnected = false;
            }
            return Communicator.Close();
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ( {2} )", Description, Name, Communicator.ToString());
        }

        internal override void EnqueueInstrumentCmd(InstrumentCmd instrumentCmd)
        {
            CPC6000CommandBase cmd = instrumentCmd as CPC6000CommandBase;
            
            if (cmd == null) throw new ArgumentException($"{instrumentCmd.GetType()} не является {typeof(CPC6000CommandBase)}");

            cmd.CPC = this;
            
            base.EnqueueInstrumentCmd(instrumentCmd);
        }
    }
}