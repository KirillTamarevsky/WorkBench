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
        internal string SetActiveChannel(int channelNumber)
        {
            if (channelNumber < 1 | channelNumber > 8)
            {
                throw new ArgumentOutOfRangeException($"Номер канала ({channelNumber}) вне допустимого диапазона (1...8) ! ");
            }
            
            //TODO check if elmetro kelvin actually selected desired channel
            
            return Communicator.QueryCommand($"CHAN {channelNumber}");
        }

        internal OneMeasure Read_0_20_Current_with_ext_pwr(EKchanNum eKChannel)
        {
            SetActiveChannel((int)eKChannel);
            double.TryParse(Communicator.QueryCommand("CURR?").Replace(".", ","), out double result);
            return new OneMeasure(result, new mA(), DateTime.Now);
        }
        //------------------------------------------------------------------------------------------
        #endregion
    }
}
