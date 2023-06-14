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
        private void btn_ReadHART_Scale_Click(object sender, EventArgs e)
        {
            var selectedHartPortName = cb_HART_SerialPort.SelectedItem;

            if (selectedHartPortName!= null && Factory.IsSerialPortPresentInSystem(selectedHartPortName.ToString()))
            {
                btn_ReadHART_Scale.Enabled = false;

                btn_HART_ZEROTRIM.Enabled = false;
                Task.Run(() =>
                {
                    var hartcomm = new HartCommunicationLite(selectedHartPortName.ToString())
                    {
                        AutomaticZeroCommand = true
                    };
                    try
                    {
                        var openres = hartcomm.Open();
                        if (openres == OpenResult.Opened)
                        {
                            var zeroCommandRes = hartcomm.SendZeroCommand();
                            if (zeroCommandRes != null)
                            {
                                var commres = hartcomm.Send(15);
                                var scaleMax = BitConverter.ToSingle(new byte[] { commres.Data[6], commres.Data[5], commres.Data[4], commres.Data[3] }, 0);
                                var scaleMin = BitConverter.ToSingle(new byte[] { commres.Data[10], commres.Data[9], commres.Data[8], commres.Data[7] }, 0);
                                var step = (scaleMax - scaleMin) / 4;
                                setTextBoxText(scaleMax.ToString("N4"), tbScaleMax);
                                setTextBoxText(scaleMin.ToString("N4"), tbScaleMin);
                                setTextBoxText(step.ToString("N4"), tb_cpcStep);
                                setTextBoxText(scaleMin.ToString("N4"), tb_PressureSetPoint);
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
                    }
                    catch (Exception)
                    {
                    }
                    finally { hartcomm.Close(); }
                    InvokeControlAction(btn_ReadHART_Scale, () => btn_ReadHART_Scale.Enabled = true);
                    InvokeControlAction(btn_HART_ZEROTRIM, () => btn_HART_ZEROTRIM.Enabled = true);
                });
            }
        }

        private void btn_HART_ZEROTRIM_Click(object sender, EventArgs e)
        {
            var selectedHartPortName = cb_HART_SerialPort.SelectedItem;

            if (selectedHartPortName!= null && Factory.IsSerialPortPresentInSystem(selectedHartPortName.ToString()))
            {
                btn_ReadHART_Scale.Enabled = false;
                btn_HART_ZEROTRIM.Enabled = false;
                Task.Run(() => {
                    var hartcomm = new HartCommunicationLite(selectedHartPortName.ToString())
                    {
                        AutomaticZeroCommand = true
                    };
                    try
                    {
                        var openres = hartcomm.Open();
                        if (openres == OpenResult.Opened)
                        {
                            var zeroCommandRes = hartcomm.SendZeroCommand();
                            if (zeroCommandRes != null)
                            {
                                hartcomm.Send(43);
                            }
                        }

                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        hartcomm.Close();
                    }
                    InvokeControlAction(btn_ReadHART_Scale, () => btn_ReadHART_Scale.Enabled = true);
                    InvokeControlAction(btn_HART_ZEROTRIM, () => btn_HART_ZEROTRIM.Enabled = true);
                });
            }

        }


    }
}
