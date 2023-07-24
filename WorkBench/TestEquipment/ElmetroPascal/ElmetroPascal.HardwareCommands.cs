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

        internal bool SwitchToREMOTEMode()
        {
            var replyStatus = Communicator.QueryCommand("R", out string reply);
            if (replyStatus == Communicators.TextCommunicatorQueryCommandStatus.Success && reply.Contains("REMOTE"))
            {
                _in_REMOTE_mode = true;
            }
            return _in_REMOTE_mode == true;
        }

        internal bool SwitchToLOCALMode()
        {
            var replyStatus = Communicator.QueryCommand("LOCAL", out string reply);
            if (replyStatus == Communicators.TextCommunicatorQueryCommandStatus.Success && reply.Contains("OK"))
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
