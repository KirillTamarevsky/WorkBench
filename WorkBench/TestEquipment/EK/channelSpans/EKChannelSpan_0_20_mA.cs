using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
//using WorkBench.AbstractClasses.InstrumentChannel;
//using WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EK.channelSpans
{
    public class EKChannelSpan_0_20_mA : IInstrumentChannelSpanReader
    {
        private OneMeasure LastValue { get; set; }

        public EKChannel parentChannel { get; }

        public Scale Scale { get; }

        public EKChannelSpan_0_20_mA(EKChannel parent)
        {
            parentChannel = parent;
            Scale = new Scale(0, 20, new mA());
            LastValue = new OneMeasure(0, new mA());
        }
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
                    var ekReplyStatus = parentChannel.ParentEK.Communicator.QueryCommand("CURR?", out string ekReply);
                    double.TryParse(ekReply.Replace(".", ","), out double result);
                    
                    LastValue = new OneMeasure(result, new mA(), DateTime.Now);
                    readingnow = false;
                }
            }
            return LastValue;
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
