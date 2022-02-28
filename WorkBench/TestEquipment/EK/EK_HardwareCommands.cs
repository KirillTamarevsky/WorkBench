using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.EK
{
    partial class EK
    {
        #region EK hardware interface commands
        //------------------------------------------------------------------------------------------
        internal string SetCurrentChannel(int channelNumber)
        {
            return _communicator.QueryCommand(string.Format("CHAN {0}", channelNumber));
        }

        internal OneMeasureResult Read_0_20_Current_with_ext_pwr(EKchanNum eKChannel)
        {
            SetCurrentChannel((int)eKChannel);
            double result;
            double.TryParse(_communicator.QueryCommand("CURR?").Replace(".", ","), out result);
            return new OneMeasureResult() { Value = result, UOM = new UOM.mA(), dateTimeOfMeasurement = DateTime.Now };
        }
        //------------------------------------------------------------------------------------------
        #endregion
    }
}
