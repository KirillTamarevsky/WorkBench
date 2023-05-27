using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EK
{
    partial class EK // : AbstractClasses.Instrument.AbstractInstrument
    {
        #region EK hardware interface commands
        //------------------------------------------------------------------------------------------
        private EKchanNum activeChannel;
        internal string SetActiveChannel(EKchanNum channelNumber)
        {
            if (channelNumber == EKchanNum.None)
            {
                activeChannel = EKchanNum.None;
                return ("0");
            }
            if (activeChannel != channelNumber)
            {
                //TODO check if elmetro kelvin actually selected desired channel
                activeChannel = channelNumber;
                return Communicator.QueryCommand($"CHAN {(int)channelNumber}");
            }
            return ((int)channelNumber).ToString();
        }

        //internal OneMeasure Read_0_20_Current_with_ext_pwr(EKchanNum eKChannel)
        //{
        //    SetActiveChannel(eKChannel);
        //    double.TryParse(Communicator.QueryCommand("CURR?").Replace(".", ","), out double result);
        //    return new OneMeasure(result, new mA(), DateTime.Now);
        //}
        //------------------------------------------------------------------------------------------
        #endregion
    }
}
