﻿using System;
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
using WorkBench.Communicators;

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
                , new EKChannelSpan_Pt100_1385_100Ohm(this)
            }.ToArray();

        }

        public override string ToString()
        {
            return Name;
        }

        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer) => ParentEK.Query(cmd, out answer);
        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer, Func<string, bool> validationRule) => ParentEK.Query(cmd, out answer, validationRule);
    }
}
