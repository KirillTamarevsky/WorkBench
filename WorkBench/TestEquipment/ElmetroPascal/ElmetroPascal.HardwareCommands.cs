using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    partial class ElmetroPascal
    {
        #region Elmetro Pascal hardware interface commands
        //------------------------------------------------------------------------------------------

        internal bool SwitchToREMOTEMode ()
        {
            var res = Communicator.QueryCommand("R");
            if (!string.IsNullOrEmpty(res) && res.Contains("REMOTE"))
            {
                _in_REMOTE_mode = true;
            }
            return _in_REMOTE_mode == true;

        }

        internal bool SwitchToLOCALMode()
        {
            var res = Communicator.QueryCommand("LOCAL");
            if (!string.IsNullOrEmpty(res) && res.Contains("OK"))
            {
                _in_REMOTE_mode = false;
                return true;
            }
            return false;
        }

        //------------------------------------------------------------------------------------------
        #endregion
    }
}
