using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.Calys150
{
    internal class Calys150_4_20Current_ReaderSpan : IInstrumentChannelSpanReader
    {
        Calys150ReaderChannel parentChannel { get; }
        private string Query(string command) => parentChannel.Query(command);
        public Scale Scale { get; } = new Scale(4, 20, new mA());
        public Calys150_4_20Current_ReaderSpan( Calys150ReaderChannel calys150ReaderChannel)
        {
                parentChannel = calys150ReaderChannel;
        }
        public OneMeasure Read(IUOM uom)
        {
            if (uom.UOMType != Enums.UOMType.Current) throw new ArgumentException();
            var reply = Query($"MEAS{parentChannel.NUM}:CURR? 25MA");
            if (reply.Length > 0)
            {
                var replyParts = reply.Split(",");
                if (replyParts.Length == 2 && replyParts[1].Trim().ToUpper() == "MA")
                {
                    var measuredValue = double.Parse(replyParts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
                    return new OneMeasure(measuredValue, new mA());
                }
            }
            return new OneMeasure(double.NaN, new mA());
        }

        public void Zero(){}
    }
}
