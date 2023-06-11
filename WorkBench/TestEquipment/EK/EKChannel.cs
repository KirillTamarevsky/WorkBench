using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
//using WorkBench.AbstractClasses.InstrumentChannel;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK.channelSpans;
using WorkBench.UOMS;
//using WorkBench.AbstractClasses.Instrument;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.EK
{
    public class EKChannel : IInstrumentChannel
    {
        public EKchanNum EKchanNum { get; }

        #region AbstractInstrumentChannel

        public int NUM => (int)EKchanNum;
        public string Name => $"{ParentEK.Name}({ParentEK.Communicator}), канал {NUM}";

        public EK ParentEK{ get; }
        public IInstrumentChannelSpan[] AvailableSpans { get; }

        #endregion

        public EKChannel(EK parent, EKchanNum chanNum)
        {
            ParentEK = parent;
            
            EKchanNum = chanNum;

            AvailableSpans = new List<IInstrumentChannelSpan>()
            {
                new EKChannelSpan_0_20_mA(this)
            }.ToArray();

        }

        public override string ToString()
        {
            return Name;
        }

    }
}
