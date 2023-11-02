using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommonTables
{
     public enum _009_Burst_Mode_Control_Code
    {
        Off = 0,
        Enable_on_Token_Passing_Data_Link_Layer_only = 1,
        Enable_on_TDMA_Data_Link_only = 2,
        Enable_on_TDMA_and_Token_Passing_Data_Link_Layers = 3,
        Enable_on_HART_IP_connection = 4

    }
}
