using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Communication.HartLite;
using WorkBench;
using WorkBench.Interfaces;

namespace benchGUI
{
    public partial class MainForm
    {
        private HartCommunicationLite hart_communicator { get; set; }
        private void btn_HART_open_Click(object sender, EventArgs e)
        {
            btn_HART_open.Enabled = false;
            if (hart_communicator == null)
            {
                HARTCommunicator_Open();
            }
            else
            {
                HARTCommunicator_Close();
            }
            btn_HART_open.Enabled = true;
        }

        private void HARTCommunicator_Open()
        {
            var selectedHartPortName = cb_HART_SerialPort.Text?.ToString();
            if ( !string.IsNullOrEmpty( selectedHartPortName ))
            {

                hart_communicator = new HartCommunicationLite(selectedHartPortName);
                var openRes = hart_communicator.Open();
                switch (openRes)
                {
                    case OpenResult.Opened:
                        btn_HART_open.Text = "Закрыть";
                        btn_HART_ZEROTRIM.Enabled = true;
                        btn_ReadHART_Scale.Enabled = true;
                        btn_HART_set_4mA.Enabled = true;
                        btn_HART_trim20mA.Enabled = true;
                        btn_HART_set_0mA.Enabled = true;
                        break;
                    case OpenResult.ComPortIsOpenAlreadyOpen:
                    case OpenResult.ComPortNotExisting:
                    case OpenResult.UnknownComPortError:
                        hart_communicator.Close();
                        break;
                    default:
                        break;
                }
            }

        }
        private void HARTCommunicator_Close()
        {
            var closeRes = hart_communicator.Close();
            hart_communicator = null;
            btn_HART_open.Text = "Открыть";
            btn_HART_ZEROTRIM.Enabled = false;
            btn_ReadHART_Scale.Enabled = false;
            btn_HART_set_4mA.Enabled = false;
            btn_HART_trim20mA.Enabled = false;
            btn_HART_set_0mA.Enabled = false;
        }


        private void btn_ReadHART_Scale_Click(object sender, EventArgs e)
        {
            btn_ReadHART_Scale.Enabled = false;
            btn_HART_ZEROTRIM.Enabled = false;
            
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    var zeroCommandRes = hart_communicator.SendZeroCommand();
                    if (zeroCommandRes != null)
                    {
                        var commres = hart_communicator.Send(15);
                        var scaleMax = BitConverter.ToSingle(new byte[] { commres.Data[6], commres.Data[5], commres.Data[4], commres.Data[3] }, 0);
                        var scaleMin = BitConverter.ToSingle(new byte[] { commres.Data[10], commres.Data[9], commres.Data[8], commres.Data[7] }, 0);
                        var step = (scaleMax - scaleMin) / 4;
                        InvokeControlAction(tbScaleMax, () => tbScaleMax.Text = scaleMax.ToString("N4"));
                        InvokeControlAction(tbScaleMin, () => tbScaleMin.Text = scaleMin.ToString("N4"));
                        InvokeControlAction(tb_cpcStep, () => tb_cpcStep.Text = step.ToString("N4"));
                        InvokeControlAction(tb_PressureSetPoint, () => tb_PressureSetPoint.Text = scaleMin.ToString("N4"));
                        switch (commres.Data[2])
                        {
                            case 7: //7 bars
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "bar").FirstOrDefault());
                                break;
                            case 8: //8 millibars
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "mbar").FirstOrDefault());
                                break;
                            case 11: //11 pascals
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "Pa").FirstOrDefault());
                                break;
                            case 12: //12 kilopascals
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "kPa").FirstOrDefault());
                                break;
                            case 237: //237 megapascals
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "MPa").FirstOrDefault());
                                break;

                            case 32: //32 Degrees Celsius
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "°C").FirstOrDefault());
                                break;
                            case 33: //33 Degrees Fahrenheit
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "°F").FirstOrDefault());
                                break;
                            case 34: //34 Degrees Rankine
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "°R").FirstOrDefault());
                                break;
                            case 35: //35 Kelvin
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "°K").FirstOrDefault());
                                break;
                            case 239: //239 millimeters of water at 4 degrees C
                            case 4: //4 millimeters of water at 68 degrees F
                                setComboboxSelectedItem(cbPressureScaleUOM, cbPressureScaleUOM.Items.Cast<IUOM>().Where(u => u.Name == "mmH2O@4°C").FirstOrDefault());
                                break;
                            default:
                                setComboboxSelectedItemIndex(cbPressureScaleUOM, -1);
                                break;
                        }
                    }
                }
                InvokeControlAction(btn_ReadHART_Scale, () => btn_ReadHART_Scale.Enabled = true);
                InvokeControlAction(btn_HART_ZEROTRIM, () => btn_HART_ZEROTRIM.Enabled = true);
            });
        }

        private void btn_HART_ZEROTRIM_Click(object sender, EventArgs e)
        {
            btn_ReadHART_Scale.Enabled = false;
            btn_HART_ZEROTRIM.Enabled = false;
            Task.Run(() => 
            {
                lock (hart_communicator)
                {
                    var zeroCommandRes = hart_communicator .SendZeroCommand();
                    if (zeroCommandRes != null) hart_communicator.Send(43);
                }

                InvokeControlAction(btn_ReadHART_Scale, () => btn_ReadHART_Scale.Enabled = true);
                InvokeControlAction(btn_HART_ZEROTRIM, () => btn_HART_ZEROTRIM.Enabled = true);
            });

        }

        private void btn_HART_set_4mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(4f);
            btn_HART_set_4mA.Enabled = true;
            btn_HART_set_20mA .Enabled = false;
        }
        private void btn_HART_set_20mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(20f);
            btn_HART_set_4mA.Enabled = false;
            btn_HART_set_20mA.Enabled = true;
        }
        private void btn_HART_set_0mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(0f);
            btn_HART_set_4mA.Enabled = false;
            btn_HART_set_20mA.Enabled = false;
        }

        private void SetHart_mA_level(Single mAlevel)
        {
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    hart_communicator.Send(40, Single_to_HART_bytearray(mAlevel));
                }
            });
        }

        private void btn_HART_trim4mA_Click(object sender, EventArgs e)
        {
            if (currentStabilityCalc.TrendStatus == WorkBench.Enums.TrendStatus.Stable)
            {
                var currReading = currentStabilityCalc.MeanValue;
                Task.Run(() =>
                {
                    lock (hart_communicator)
                    {
                        hart_communicator.Send(45, Single_to_HART_bytearray((float)currReading));
                    }
                });
            }

        }

        private void btn_HART_trim20mA_Click(object sender, EventArgs e)
        {
            if (currentStabilityCalc.TrendStatus == WorkBench.Enums.TrendStatus.Stable)
            {
                var currReading = currentStabilityCalc.MeanValue;
                Task.Run(() =>
                {
                    lock (hart_communicator)
                    {
                        hart_communicator.Send(46, Single_to_HART_bytearray((float)currReading));
                    }
                });
            }
        }

        private byte[] Single_to_HART_bytearray(Single number)
        {
            var bytes = BitConverter.GetBytes(number);
            var data = new byte[4] { bytes[3], bytes[2], bytes[1], bytes[0] };
            return data;
        }
    }
}
