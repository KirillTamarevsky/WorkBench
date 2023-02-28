using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.AbstractClasses.InstrumentChannel;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK.channelSpans;
using WorkBench.UOMS;
using WorkBench.AbstractClasses.Instrument;

namespace WorkBench.TestEquipment.EK
{
    public class EKChannel : AbstractInstrumentChannel
    {
        public EKchanNum eKchanNum { get; internal set; }

        #region AbstractInstrumentChannel

        private int _num;
        public override int NUM
        {
            get
            {
                return _num;
            }
            protected internal set
            {
                if (value < 1 | value > 8) throw new ArgumentException($"номер канала ({value}) вне допустимого диапазона (1...8) !");

                _num = value;
            }
        }
        public override string Name
        {
            get
            {
                return $"{parent.Name}, канал {NUM}";
            }
            protected internal set { }
        }

        private EK _parent;
        public override AbstractInstrument parent
        {
            get
            {
                return _parent;
            }
            protected internal set
            {
                var p = value as EK;
                if (p == null)
                {
                    throw new ArgumentException(string.Format("{0} is not {1}", value.GetType().ToString(), typeof(EK).ToString()));
                }
                _parent = p;
            }
        }


        #endregion

        public EKChannel()
        {
            var _AvailableSpans = new List<WorkBench.Interfaces.InstrumentChannel.IInstrumentChannelSpan>();

            _AvailableSpans.Add(new EKChannelSpan_0_20_mA()
            {
                //CyclicRead = false,
                parentChannel = this,
                Scale = new Scale(0, 20, new mA()),
                LastValue = new OneMeasure(0, new mA(), DateTime.Now)
            });

            AvailableSpans = _AvailableSpans.ToArray();
        }



        public override string ToString()
        {
            return Name;
        }

    }
}
