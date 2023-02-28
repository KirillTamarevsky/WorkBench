using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;
using WorkBench.AbstractClasses.InstrumentChannel;
using WorkBench.AbstractClasses.Instrument;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public class ElmetroPascalChannel : AbstractInstrumentChannel
    {
        public ElmetroPascal _parent { get; protected internal set; }


        #region AbstractInstrumentChannel
        public override int NUM 
        {
            protected internal set { }
            get
            {
                return 1;
            }
        }

        private string _name;
        public override string Name
        {
            protected internal set
            {
                _name = value;
            }

            get
            {
                return _name;
            }
        }

        public override AbstractInstrument parent 
        { 
            get
            {
                return _parent;
            }
            protected internal set
            {
                var ep = value as ElmetroPascal;
                if (ep == null)
                {
                    throw new ArgumentException(string.Format("{0} is not {1}", value.GetType().Name, typeof(ElmetroPascal).Name));
                }
                _parent = ep;
            }
        }

        #endregion


        public ElmetroPascalChannel(ElmetroPascal elmetroPascal)
        {
            if (elmetroPascal == null) throw new ArgumentNullException($"Элметро-Паскаль = NULL");

            parent = elmetroPascal;

            Name = "канал давления";
            //setup Channel Spans
            var availableSpans = ((ElmetroPascal)parent).UsableRangesAsync();

            var _avSpans = new List<ElmetroPascalChannelSpan>();

            foreach (var span in availableSpans)
            {
                var avspan = 
                    new ElmetroPascalChannelSpan() 
                    { 
                        parentChannel = this, 

                        //SetPressureOperationMode(PressureControllerOperationMode.UNKNOWN),

                        Scale = span
                    };
                avspan.SetPressureOperationMode(PressureControllerOperationMode.UNKNOWN);
                _avSpans.Add(avspan);
            }

            AvailableSpans = _avSpans.ToArray();
        }

        public override string ToString()
        {
            var min = AvailableSpans.OrderBy(a => a.Scale.Min).Select(a => a.Scale.Min).FirstOrDefault();
            
            var max = AvailableSpans.OrderByDescending(a => a.Scale.Max).Select(a => a.Scale.Max).FirstOrDefault();

            return String.Format("канал 1: {0} - {1} {2}", min, max, "kPa");
        }


    }
}
