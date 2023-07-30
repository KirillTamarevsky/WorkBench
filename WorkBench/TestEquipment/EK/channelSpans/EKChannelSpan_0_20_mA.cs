using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Communicators;
//using WorkBench.AbstractClasses.InstrumentChannel;
//using WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EK.channelSpans
{
    public class EKChannelSpan_0_20_mA : IInstrumentChannelSpanReader
    {
        private OneMeasure? LastReadedValue { get; set; }

        public EKChannel parentChannel { get; }

        public Scale Scale { get; }

        public EKChannelSpan_0_20_mA(EKChannel parent)
        {
            parentChannel = parent;
            Scale = new Scale(0, 20, new mA());
            LastReadedValue = null; // new OneMeasure(0, new mA());
        }
        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer) => parentChannel.Query(cmd, out answer);
        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer, Func<string, bool> validationRule) => parentChannel.Query(cmd, out answer, validationRule);
        private bool readingnow;
        public OneMeasure Read(IUOM uom)
        {
            var ek = parentChannel.ParentEK;
            var skipActualReading = readingnow == true;
            lock (parentChannel.ParentEK.Communicator)
            {
                if (!skipActualReading)
                {
                    readingnow = true;

                    ek.SetActiveChannel(this.parentChannel.EKchanNum);
                    //--------------Read_0_20_Current_with_ext_pwr-------------------
                    //CURR?
                    //Измерение тока в режиме «0 - 20».
                    //В случае успешного выполнения команды возвращается результат измерения(мА).
                    //В противном случае возвращается «ERROR».
                    //Пример
                    //Команда: CURR?
                    //Ответ: 2.0501
                    //
                    //TODO добавить проверку ответа на ERROR
                    var validationRule = (string s) =>
                    {
                        return double.TryParse(s.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out _);
                    };
                    var ekReplyStatus = Query("CURR?", out string ekReply, validationRule);
                    if (ekReplyStatus == TextCommunicatorQueryCommandStatus.Success)
                    {
                        var result = double.Parse(ekReply.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);
                        LastReadedValue = new OneMeasure(result, new mA(), DateTime.Now);
                    }
                    else
                    {
                        LastReadedValue = null;
                    }
                    
                    readingnow = false;
                }
            }
            return LastReadedValue;
        }

        public override string ToString()
        {
            return $"{parentChannel.Name} измерение тока {Scale}, внешний источник питания";
        }

        public void Zero()
        {
            throw new NotImplementedException("EK channel 0 - 20 mA zeriong not implemented");
        }

    }
}
