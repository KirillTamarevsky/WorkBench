using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{
    public class HART_Result_015_Read_Device_Information : CommandResult
    {
        internal HART_Result_015_Read_Device_Information(HARTDatagram command) : base(command)
        {
        }

        public byte PVAlarmSelectionCode => Data[0];
        public byte PVTransferFunctionCode => Data[1];
        public byte PVUpperAndLowerRangeValuesUnitCode => Data[2];
        public Single PVUpperRangeValue => new byte[] { Data[3], Data[4], Data[5], Data[6] }.To_HART_Single();
        public Single PVLowerRangeValue => new byte[] { Data[7], Data[8], Data[9], Data[10] }.To_HART_Single();
        public Single PVDampingValue_in_seconds => new byte[] { Data[11], Data[12], Data[13], Data[14] }.To_HART_Single();
        public byte WriteProtectCode => Data[15];
        public byte PVAnalogChannelFlags => Data[17];

    }
}
