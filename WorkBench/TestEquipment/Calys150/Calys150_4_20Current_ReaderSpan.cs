using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkBench.Communicators;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.Calys150
{
    internal abstract class Calys150_Current_ReaderSpan_base : IInstrumentChannelSpanReader
    {
        internal Calys150ReaderChannel parentChannel { get; }
        ITextCommunicator Communicator => parentChannel.Communicator;
        void Activate() => parentChannel.ActivateSpan(this);
        int ChanNum { get => parentChannel.NUM; }
        private TextCommunicatorQueryCommandStatus Query(string command, out string answer) => parentChannel.Query(command, out answer);
        public abstract Scale Scale { get; }
        public abstract string queryingRangeString { get; }
        public string SetupStringCommand => $"CURR:RANGE {queryingRangeString}; SUPPLY ON; HART ON";

        public Calys150_Current_ReaderSpan_base( Calys150ReaderChannel calys150ReaderChannel)
        {
                parentChannel = calys150ReaderChannel;
        }
        public OneMeasure Read(IUOM uom)
        {
            lock (Communicator)
            {
                Activate();
                if (uom.UOMType != Enums.UOMType.Current) throw new ArgumentException("only current measurement supported on this span");
                var measuredValue = double.NaN;
                var replyStaus = Query($"MEAS{ChanNum}?", out string reply);
                if (reply.Length > 0)
                {
                    var replyParts = reply.Split(",");
                    if (replyParts.Length == 2)
                    {
                        double.TryParse(replyParts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out measuredValue);
                    }
                }
                return new OneMeasure(measuredValue, new mA());
            }
        }

        public void Zero(){}
    }

    internal class Calys150_0Current_ReaderSpan : Calys150_Current_ReaderSpan_base
    {
        public override Scale Scale { get; } = new Scale(0, 100, new mA());
        public override string queryingRangeString => "0MA";
        public Calys150_0Current_ReaderSpan(Calys150ReaderChannel calys150ReaderChannel) : base(calys150ReaderChannel) { }
        public override string ToString() => "0 - 20 mA; pwr on; hart on";

    }
    internal class Calys150_4Current_ReaderSpan : Calys150_Current_ReaderSpan_base
    {
        public override Scale Scale { get; } = new Scale(0, 100, new mA());
        public override string queryingRangeString => "4MA";
        public Calys150_4Current_ReaderSpan(Calys150ReaderChannel calys150ReaderChannel) : base(calys150ReaderChannel) { }
        public override string ToString() => "4 - 20 mA; pwr on; hart on";

    }
    internal class Calys150_0_100Current_ReaderSpan : Calys150_Current_ReaderSpan_base
    {
        public override Scale Scale { get; } = new Scale(0, 100, new mA());
        public override string queryingRangeString => "100MA";
        public Calys150_0_100Current_ReaderSpan(Calys150ReaderChannel calys150ReaderChannel) : base(calys150ReaderChannel) { }
        public override string ToString() => "0 - 100 mA; pwr on; hart on";
    }

}
