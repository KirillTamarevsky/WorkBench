using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EK
{
    partial class EK // : AbstractClasses.Instrument.AbstractInstrument
    {
        #region EK hardware interface commands
        //------------------------------------------------------------------------------------------
        private EKchanNum activeChannel;
        internal EKchanNum SetActiveChannel(EKchanNum channelNumber)
        {
            if (channelNumber == EKchanNum.None)
            {
                activeChannel = EKchanNum.None;
                return activeChannel;
            }
            if (activeChannel != channelNumber)
            {
                //TODO check if elmetro kelvin actually selected desired channel
                var intChannelNumber = (int)channelNumber;
                var validationRule = (string s) =>
                {
                    if (int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int actualChanNumber)) 
                    {
                        return actualChanNumber == intChannelNumber;
                    }
                    return false;
                };
                
                var answerStatus = Query($"CHAN {(int)channelNumber}", out string reply, validationRule);
                if (answerStatus == Communicators.TextCommunicatorQueryCommandStatus.Success)
                {
                    int actChanNumber = int.Parse(reply.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    activeChannel = (EKchanNum)actChanNumber;
                }
                else
                {
                    activeChannel = EKchanNum.None;
                }
            }
            return activeChannel;
        }

        //internal OneMeasure Read_0_20_Current_with_ext_pwr(EKchanNum eKChannel)
        //{
        //    SetActiveChannel(eKChannel);
        //    double.TryParse(Communicator.QueryCommand("CURR?").Replace(".", ","), out double result);
        //    return new OneMeasure(result, new mA(), DateTime.Now);
        //}
        //------------------------------------------------------------------------------------------
        #endregion
    }
}
