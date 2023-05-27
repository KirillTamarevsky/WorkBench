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
//using WorkBench.TestEquipment.CPC6000;
using WorkBench.TestEquipment.ElmetroPascal;
//using WorkBench.TestEquipment.EVolta;

namespace WorkBench
{
    public class Factory
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Factory));

        static public EK GetFakeEK (string portName)
        {
            var fakeEKserialPort = new FakeEKSerialPort(portName, 115200, Parity.Odd, 8, StopBits.One);
            var communicator = new SerialEKCommunicator(fakeEKserialPort, "\r\n");

            var ek = new EK(communicator);

            return ek;
        }

        static public EK GetEK_on_SerialPort(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            EK ek = null;

            if (IsSerialPortPresentInSystem(serialPortName))
            {
                var serialPort = new WBSerialPortWrapper(serialPortName, baudrate, parity, dataBits, stopBits);
                var communicator = new SerialEKCommunicator(serialPort, "\r\n");

                ek = new EK(communicator);

            }
            return ek;
        }
        static public EK GetEK_on_SerialPort_with_default_Port_Settings(string portName)
        {
            return GetEK_on_SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        }
        //static public IInstrument GetEVolta_on_SerialPort_with_default_Port_Settings(string portName)
        //{
        //    EVolta evolta = null;

        //    if (IsSerialPortPresentInSystem(portName))
        //    {
        //        var communicator = new SerialEKCommunicator(portName, 9600, Parity.None, 8, StopBits.One, "\r\n");

        //        evolta = new EVolta(communicator);

        //    }
        //    return evolta;
        //}

        //static public IInstrument GetFakeEVolta(string portName)
        //{
        //    var communicator = new FakeEVoltaCommunicator(portName);

        //    var evolta = new EVolta(communicator);

        //    return evolta;
        //}
        static public ElmetroPascal GetFakeEPascal(string portName)
        {
            var fakeEPascalSerialPort = new FakeEPascalSerialPort(portName, 115200, Parity.Odd, 8, StopBits.One);
            var communicator = new SerialEKCommunicator(fakeEPascalSerialPort, "\r\n");

            var ek = new ElmetroPascal(communicator);

            return ek;
        }
        static public ElmetroPascal GetEPascal_on_SerialPort(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            ElmetroPascal ep = null;

            if (IsSerialPortPresentInSystem(serialPortName))
            {
                var serialPort = new WBSerialPortWrapper(serialPortName, baudrate, parity, dataBits, stopBits);

                var communicator = new SerialEKCommunicator(serialPort, "\r\n");

                ep = new ElmetroPascal(communicator);

            }

            return ep;
        }
        static public ElmetroPascal GetEPascal_on_SerialPort_with_default_Port_Settings(string portName)
        {
            return GetEPascal_on_SerialPort(portName, 19200, Parity.Odd, 8, StopBits.One);
        }
        //static public IInstrument GetCPC6000_on_Fake_SerialPort()
        //{
        //    var fakeSerialPortWrapper = new FakeCPC6000SerialPort("COM111", 57600, Parity.None, 8, StopBits.One);
        //    var communicator = new SerialCPC6000Communicator(fakeSerialPortWrapper);

        //    var cpc = new CPC6000(communicator);

        //    return cpc;
        //}
        //static public IInstrument GetCPC6000_on_SerialPort(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        //{
        //    CPC6000 cpc = null;

        //    if (IsSerialPortPresentInSystem(serialPortName))
        //    {
        //        var serialPortWrapper = new WBSerialPortWrapper(serialPortName, baudrate, parity, dataBits, stopBits);
        //        var communicator = new SerialCPC6000Communicator(serialPortWrapper);

        //        cpc = new CPC6000(communicator);

        //    }

        //    return cpc;
        //}

        //static public IInstrument GetCPC6000_on_SerialPort_with_default_Port_Settings(string portName)
        //{
        //    return GetCPC6000_on_SerialPort(portName, 57600, Parity.None, 8, StopBits.One);
        //}


        /// <summary>
        /// опрашивает все последовательные порты в системе на предмет наличия за ними Элметро-Кельвины.
        /// используются стандартные настройки порта: 115200, Parity.None, 8 dataBits, StopBits.One.
        /// </summary>
        /// <returns>возвращает List string всех последовательных портов, за которыми найдены Элметро-Кельвины</returns>
        public static async Task< List<string>> serialPortNamesWithEK()
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
                await Task.WhenAll(searchTasks).ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled);
            
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
        
        public static bool IsSerialPortPresentInSystem(string serialPortName)
        {
            return GetSerialPortsNames().Contains(serialPortName);
        }
        
        static public string[] GetSerialPortsNames()
        {
            return SerialPort.GetPortNames();
        }
    }
}
