using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBGUIWPF.viewmodels;

namespace WBGUIWPF.Interfaces
{
    public interface IInstrumentWithSerialPort
    {
        SerialPortConfigurationVM? SelectedSerialPortConfiguration { get; set; }
        event EventHandler<SerialPortConfigurationVM> SerialPortChanged;
    }
}
