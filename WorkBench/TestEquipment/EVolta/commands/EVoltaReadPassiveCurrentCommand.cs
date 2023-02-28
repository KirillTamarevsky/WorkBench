using System;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK.commands;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EVolta.channelSpans
{
    internal class EVoltaReadPassiveCurrentCommand : EVoltaCommandBase
    {
        private Action<OneMeasure> ReportTo;

        public EVoltaReadPassiveCurrentCommand(Action<OneMeasure> reportTo)
        {
            ReportTo = reportTo;
        }

        public override void Execute()
        {
            var res = Volta.Communicator.QueryCommand("CURR?");

            if (!string.IsNullOrEmpty(res) && double.TryParse(res.Replace(".", ","), out double result))
            {
                ReportTo(new OneMeasure(result, new mA()));
            }
        }
    }
}