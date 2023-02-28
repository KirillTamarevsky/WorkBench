using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.TestEquipment;
using WorkBench.Communicators;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench
{
    class Program
    {
        static void Main(string[] args)
        {
            //var portsWithEK = Factory.serialPortsWithEK();
            if (1 >= 0)
            {
                //IInstrument ek = new EK(portsWithEK.First());
                IInstrument ek = Factory.GetFakeEK("COM8");
                //IInstrument ek = Factory.GetEK_default_SerialPort_Settings("COM3");
                foreach (IInstrumentChannel channel in ek.Channels)
                {
                    ((IInstrumentReaderChannel)channel).CyclicRead = true;
                    Console.WriteLine(channel.Name + " " + ((IInstrumentReaderChannel)channel).CanRead (new Scale() { Min = 4, Max = 20, UOM = new UOMS.mA() }));
                    ((IInstrumentReaderChannel)channel).NewValueReaded += OnChan1NewValueReaded;
                    ((IInstrumentReaderChannel)channel).Read(Enums.UOMType.Current);
                }
                ((IInstrumentReaderChannel)ek.Channels[0]).CyclicRead = true;
                ek.Open();
                char k;
                do
                {
                    k = Console.ReadKey(true).KeyChar;
                    switch (k)
                    {
                        case '1':
                            ((IInstrumentReaderChannel)ek.Channels[0]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[0]).CyclicRead;
                            ((IInstrumentReaderChannel)ek.Channels[0]).Read(Enums.UOMType.Current);
                            break;
                        case '2':
                            ((IInstrumentReaderChannel)ek.Channels[1]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[1]).CyclicRead;
                            break;
                        case '3':
                            ((IInstrumentReaderChannel)ek.Channels[2]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[2]).CyclicRead;
                            break;
                        case '4':
                            ((IInstrumentReaderChannel)ek.Channels[3]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[3]).CyclicRead;
                            break;
                        case '5':
                            ((IInstrumentReaderChannel)ek.Channels[4]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[4]).CyclicRead;
                            break;
                        case '6':
                            ((IInstrumentReaderChannel)ek.Channels[5]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[5]).CyclicRead;
                            break;
                        case '7':
                            ((IInstrumentReaderChannel)ek.Channels[6]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[6]).CyclicRead;
                            break;
                        case '8':
                            ((IInstrumentReaderChannel)ek.Channels[7]).CyclicRead = !((IInstrumentReaderChannel)ek.Channels[7]).CyclicRead;
                            break;

                        default:
                            break;
                    }
                } while (k != 'q') ;
                ek.Close();
            }
        }
        static void OnChan1NewValueReaded(OneMeasure onemeasure)
        {
            Console.WriteLine( onemeasure.Value + " " + onemeasure.UOM.Name);
        }
    }
}
