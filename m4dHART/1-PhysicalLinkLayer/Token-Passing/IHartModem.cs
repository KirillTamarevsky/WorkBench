using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART._1_PhysicalLinkLayer.Token_Passing
{
    public interface IHartModem
    {
        void Reset_Request();
        void ENABLE_request(bool state);
        event ENABLE_indicate_Handler ENABLE_indicate;
        event ENABLE_confirm_Handler ENABLE_confirm;
        void DATA_request(byte dataByte);
        event DATA_confirm_Handler DATA_confirm;
        event DATA_indicate_Handler DATA_indicate;


    }
    public delegate void ENABLE_indicate_Handler(bool state);
    public delegate void ENABLE_confirm_Handler(bool state);
    public delegate void DATA_confirm_Handler(byte dataByte);
    public delegate void DATA_indicate_Handler(byte dataByte);
    public delegate void ERROR_indicate_Handler(ERROR_HART_Receiving status, byte data);

    public enum ERROR_HART_Receiving
    {
        parity_error,
        framing_error,
        receive_data_overrun,
        receive_timeout
    }
}
