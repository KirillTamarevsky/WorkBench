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
    internal abstract class Calys150_Current_ReaderSpan_base : IInstrumentChannelSpanReader
    {
        Calys150ReaderChannel parentChannel { get; }
        private string Query(string command) => parentChannel.Query(command);
        public abstract Scale Scale { get; }
        public abstract string queryingRangeString { get; }
        public Calys150_Current_ReaderSpan_base( Calys150ReaderChannel calys150ReaderChannel)
        {
                parentChannel = calys150ReaderChannel;
        }
        public OneMeasure Read(IUOM uom)
        {
            if (uom.UOMType != Enums.UOMType.Current) throw new ArgumentException();
            var reply = Query($"MEAS{parentChannel.NUM}:CURR? {queryingRangeString}");
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

    internal class Calys150_4_20Current_ReaderSpan : Calys150_Current_ReaderSpan_base
    {
        public override Scale Scale { get; } = new Scale(4, 20, new mA());
        public override string queryingRangeString => "25MA";
        public Calys150_4_20Current_ReaderSpan(Calys150ReaderChannel calys150ReaderChannel) : base(calys150ReaderChannel){}
        public override string ToString() => "25MA";
    }
    internal class Calys150_0Current_ReaderSpan : Calys150_Current_ReaderSpan_base
    {
        public override Scale Scale { get; } = new Scale(0, 100, new mA());
        public override string queryingRangeString => "0MA";
        public Calys150_0Current_ReaderSpan(Calys150ReaderChannel calys150ReaderChannel) : base(calys150ReaderChannel) { }
        public override string ToString() => "0MA";

    }
    internal class Calys150_4Current_ReaderSpan : Calys150_Current_ReaderSpan_base
    {
        public override Scale Scale { get; } = new Scale(0, 100, new mA());
        public override string queryingRangeString => "4MA";
        public Calys150_4Current_ReaderSpan(Calys150ReaderChannel calys150ReaderChannel) : base(calys150ReaderChannel) { }
        public override string ToString() => "4MA";

    }
    internal class Calys150_0_100Current_ReaderSpan : Calys150_Current_ReaderSpan_base
    {
        public override Scale Scale { get; } = new Scale(0, 100, new mA());
        public override string queryingRangeString => "100MA";
        public Calys150_0_100Current_ReaderSpan(Calys150ReaderChannel calys150ReaderChannel) : base(calys150ReaderChannel) { }
        public override string ToString() => "100MA";
    }

}
