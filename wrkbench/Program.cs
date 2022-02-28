using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.TestEquipment;
using WorkBench.Communicators;

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
                foreach (IChannel channel in ek.Channels)
                {
                    ((IReader)channel).CyclicRead = true;
                    Console.WriteLine(channel.Name + " " + ((IReader)channel).CanRead (new Scale() { Min = 4, Max = 20, UOM = new UOM.mA() }));
                    ((IReader)channel).NewValueReaded += OnChan1NewValueReaded;
                    ((IReader)channel).Read(Enums.UOMType.Current);
                }
                ((IReader)ek.Channels[0]).CyclicRead = true;
                ek.Open();
                char k;
                do
                {
                    k = Console.ReadKey(true).KeyChar;
                    switch (k)
                    {
                        case '1':
                            ((IReader)ek.Channels[0]).CyclicRead = !((IReader)ek.Channels[0]).CyclicRead;
                            ((IReader)ek.Channels[0]).Read(Enums.UOMType.Current);
                            break;
                        case '2':
                            ((IReader)ek.Channels[1]).CyclicRead = !((IReader)ek.Channels[1]).CyclicRead;
                            break;
                        case '3':
                            ((IReader)ek.Channels[2]).CyclicRead = !((IReader)ek.Channels[2]).CyclicRead;
                            break;
                        case '4':
                            ((IReader)ek.Channels[3]).CyclicRead = !((IReader)ek.Channels[3]).CyclicRead;
                            break;
                        case '5':
                            ((IReader)ek.Channels[4]).CyclicRead = !((IReader)ek.Channels[4]).CyclicRead;
                            break;
                        case '6':
                            ((IReader)ek.Channels[5]).CyclicRead = !((IReader)ek.Channels[5]).CyclicRead;
                            break;
                        case '7':
                            ((IReader)ek.Channels[6]).CyclicRead = !((IReader)ek.Channels[6]).CyclicRead;
                            break;
                        case '8':
                            ((IReader)ek.Channels[7]).CyclicRead = !((IReader)ek.Channels[7]).CyclicRead;
                            break;

                        default:
                            break;
                    }
                } while (k != 'q') ;
                ek.Close();
            }
        }
        static void OnChan1NewValueReaded(IReader reader)
        {
            Console.WriteLine(((IChannel) reader).Name + " = " + reader.lastValue.Value + " " + reader.lastValue.UOM.Name);
        }
    }
}
