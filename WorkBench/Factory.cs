using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Communicators;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK;
using WorkBench.TestEquipment.CPC6000;

namespace WorkBench
{
    public class Factory
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Factory));

        static public IInstrument GetFakeEK (string portName)
        {
            var communicator = new FakeEKCommunicator(portName);

            var ek = new EK(communicator);

            return ek;
        }
        static public IInstrument GetEK_on_SerialPort(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            EK ek = null;

            if (IsSerialPortValid(serialPortName))
            {
                var communicator = new SerialEKCommunicator(serialPortName, baudrate, parity, dataBits, stopBits);

                ek = new EK(communicator);

            }
            return ek;
        }
        static public IInstrument GetEK_on_SerialPort_with_default_Port_Settings(string portName)
        {
            return GetEK_on_SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        }

        static public IInstrument GetCPC6000_on_Fake_SerialPort()
        {
            var communicator = new FakeCPC6000Communicator("COM111", 57600, Parity.None, 8, StopBits.One);

            var cpc = new CPC6000(communicator);

            return cpc;
        }
        static public IInstrument GetCPC6000_on_SerialPort(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            CPC6000 cpc = null;

            if (IsSerialPortValid(serialPortName))
            {
                var communicator = new SerialCPC6000Communicator(serialPortName, baudrate, parity, dataBits, stopBits);

                cpc = new CPC6000(communicator);

            }

            return cpc;
        }

        static public IInstrument GetCPC6000_on_SerialPort_with_default_Port_Settings(string portName)
        {
            return GetCPC6000_on_SerialPort(portName, 57600, Parity.None, 8, StopBits.One);
        }


        /// <summary>
        /// опрашивает все последовательные порты в системе на предмет наличия за ними Элметро-Кельвины.
        /// используются стандартные настройки порта: 115200, Parity.None, 8 dataBits, StopBits.One.
        /// </summary>
        /// <returns>возвращает List string всех последовательных портов, за которыми найдены Элметро-Кельвины</returns>
        public static List<string> serialPortNamesWithEK()
        {
            log4net.ILog logger = log4net.LogManager.GetLogger("Communication"); //typeof(Factory));

            List<string> res = new List<string>();

            string[] presentSerialPorts = SerialPort.GetPortNames();
            
            List<Task<string>> searchTasks = new List<Task<string>>();
            
            foreach (var port in presentSerialPorts)
            {
                searchTasks.Add(new Task<string>(
                    () =>
                {
                    logger.Debug("поиск EK на " + port + " : ");
            
                    bool opened = false;
                    
                    using (EK tryPort = (EK)GetEK_on_SerialPort(port, 115200, Parity.None, 8, StopBits.One))
                    {
                        try
                        {
                            opened = tryPort.Open();
                        }
                        catch (Exception e)
                        {
                            logger.Debug(e);
                        }

                        if (opened)
                        {
                            
                            try 
                            {
                                tryPort.Close();
                            }
                            
                            catch(Exception e)
                            {
                                logger.Debug(e);
                            }
                    
                            logger.Debug("EK найден на " + port);
                        }
                        else
                        {
                            logger.Debug("EK не найден на " + port);
                        }
                    }
                    return opened ? port : null;
                }));
            }
            logger.Debug("before searchTasks.ForEach((s) => s.Start());");
            
            searchTasks.ForEach((s) => s.Start());
            
            logger.Debug("before Task.WaitAll(searchTasks.ToArray());");
            
            try
            {
                Task.WhenAll(searchTasks).ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled).Wait();
            
                //Task.WaitAll(searchTasks.ToArray());
            }
            catch (Exception e)
            {
                logger.Debug("Task.WhenAll(searchTasks).ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled).Wait();");
            
                logger.Debug(e);
            }

            foreach (var item in searchTasks)
            {
                logger.Debug("if (!item.IsCanceled && item.Result != null) = "+ item.IsCanceled +" " + item.Result);
                
                if (!item.IsCanceled && item.Result != null)
                {
                    
                    logger.Debug(" res.Add(item.Result) " + item.Result);
                
                    res.Add(item.Result);
                }
                    
            }
            
            logger.Debug(" return res; " + res);
            
            return res;
        }
        
        static bool IsSerialPortValid(String serialPortName)
        {
            string[] presentSerialPorts = SerialPort.GetPortNames();
            
            bool spIsInSystem = false;
            
            foreach (string sp in presentSerialPorts)
            {
            
                if (sp == serialPortName)
                {
                    spIsInSystem = true;
                }
            }
            
            return spIsInSystem;
        }
        
        static public string[] GetSerialPortsNames()
        {
            return SerialPort.GetPortNames();
        }
    }
}
