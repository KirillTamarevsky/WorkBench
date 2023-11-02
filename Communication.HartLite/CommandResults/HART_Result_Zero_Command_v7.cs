using Communication.HartLite.CommonTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{
    public class HART_Result_Zero_Command_v7 : CommandResult, IReadUniqueIdetifierCommand
    {
        internal HART_Result_Zero_Command_v7(HARTDatagram command) : base(command)
        {
        }
        public byte[] ExpandedDeviceTypeCode => new byte[] { Data[1], Data[2] };
        public int MinNumberOfPreamblesRequiredFromMaster => Data[3];
        public int HARTProtocolMajorRevisionImplementedByThisDevice => Data[4];
        public int DeviceRevisionLevel => Data[5];
        public int SoftwareRevisionLevelOfThisDevice => Data[6];
        public int HardwareRevisionLevelOfTheElectronicsInThisDevice => ( Data[7] & 0b11111000 ) >> 3;
        public _010_Physical_Signaling_Code PhysicalSignalingCode => (_010_Physical_Signaling_Code)(Data[7] & 0b00000111);
        public int Flags => Data[8];
        public byte[] DeviceID => new byte[] { Data[9], Data[10], Data[11] };
        public int NumberOfPreablesToBeSentFromThisSlaveToMaster => Data[12];
        public int LastDeviceVariableCode => Data[13];
        public int ConfigurationChangeCounter => BitConverter.ToInt16(new byte[] { Data[14], Data[15] }, 0);
        public int ExtendedFieldDeviceStatus => Data[16];
        public byte[] ManufacturerIdentificationCode => new byte[] { Data[17], Data[18] };
        public byte[] PrivateLabelDistributorCode => new byte[] { Data[19], Data[20] };
        public int DeviceProfile => Data[21];

        public LongAddress LongAddress => new LongAddress(ExpandedDeviceTypeCode.Concat(DeviceID).ToArray());

    }
}
