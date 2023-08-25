using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication.HartLite;
using Communication.HartLite.Commands;
using WorkBench;
using WorkBench.Interfaces;

namespace benchGUI
{
    public partial class MainForm
    {
        IAddress HartAddr { get; set; } = new ShortAddress(0);
        private HartCommunicationLite hart_communicator { get; set; }
        Task HartBackgroundWorker { get; set; }
        CancellationTokenSource HartBackgroundWorkerCTS { get; set; }
        CancellationToken HartBackgroundWorkerCT { get; set; }
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
            if (!string.IsNullOrEmpty(selectedHartPortName))
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
                        btn_HART_set_20mA.Enabled = true;
                        btn_HART_set_0mA.Enabled = true;

                        HartBackgroundWorkerCTS = new CancellationTokenSource();
                        HartBackgroundWorkerCT = HartBackgroundWorkerCTS.Token;
                        HartBackgroundWorker = HartBackgroundWorker_DoWork();
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

        private Task HartBackgroundWorker_DoWork()
        {
            return Task.Run(() =>
            {
                while (!HartBackgroundWorkerCT.IsCancellationRequested)
                {
                    lock (hart_communicator)
                    {
                        if (HartAddr is ShortAddress)
                        {
                            if (!HartBackgroundWorkerCT.IsCancellationRequested)
                                HART_SEND_ZERO_COMMAND();

                            if (HartAddr is LongAddress)
                            {
                                if (!HartBackgroundWorkerCT.IsCancellationRequested)
                                    HART_READ_SCALE();
                                if (!HartBackgroundWorkerCT.IsCancellationRequested)
                                    HART_READ_TAG();
                            }
                        }
                        if (HartAddr is LongAddress)
                        {
                            if (!HartBackgroundWorkerCT.IsCancellationRequested)
                                HART_READ_PV_mA_SV_TV_QV();
                        }
                    }
                }
            });
        }

        private void HARTCommunicator_Close()
        {
            var closeRes = hart_communicator.Close();
            HartAddr = new ShortAddress(0);
            hart_communicator = null;
            btn_HART_open.Text = "Открыть";
            btn_HART_ZEROTRIM.Enabled = false;
            btn_ReadHART_Scale.Enabled = false;
            btn_HART_set_0mA.Enabled = false;
            btn_HART_set_4mA.Enabled = false;
            btn_HART_set_20mA.Enabled = false;
            btn_HART_trim4mA.Enabled = false;
            btn_HART_trim20mA.Enabled = false;
            if (HartBackgroundWorker != null)
            {
                HartBackgroundWorkerCTS.Cancel();
                HartBackgroundWorker.Wait();
                HartBackgroundWorkerCTS.Dispose();
                HartBackgroundWorkerCTS = null;
                HartBackgroundWorker.Dispose();
                HartBackgroundWorker = null;
            }
        }


        private void btn_ReadHART_Scale_Click(object sender, EventArgs e)
        {
            btn_ReadHART_Scale.Enabled = false;
            btn_HART_ZEROTRIM.Enabled = false;

            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    HART_READ_SCALE();
                }
                InvokeControlAction( () =>
                {
                    btn_ReadHART_Scale.Enabled = true;
                    btn_HART_ZEROTRIM.Enabled = true;
                });
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
                    var cmd = new HARTCommand(43, new byte[0]);
                    SendHARTCommand(cmd);
                }
            }).ContinueWith((t) =>
            {
                InvokeControlAction( () =>
                {
                    btn_ReadHART_Scale.Enabled = true;
                    btn_HART_ZEROTRIM.Enabled = true;
                });
            });

        }

        private void btn_HART_set_4mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(4f).ContinueWith((t) =>
            {
                InvokeControlAction( () =>
                {
                    btn_HART_trim4mA.Enabled = true;
                    btn_HART_trim20mA.Enabled = false;
                });
            });
        }
        private void btn_HART_set_20mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(20f).ContinueWith((t) =>
            {
                InvokeControlAction( () =>
                {
                    btn_HART_trim4mA.Enabled = false;
                    btn_HART_trim20mA.Enabled = true;
                });
            });
        }
        private void btn_HART_set_0mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(0f).ContinueWith((t) =>
            {
                InvokeControlAction( () =>
                {
                    btn_HART_trim4mA.Enabled = false;
                    btn_HART_trim20mA.Enabled = false;
                });
            }
                );
        }

        private Task SetHart_mA_level(Single mAlevel)
        {
            var cmd = new HART_Simulate_Current_Command(mAlevel);
            return Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    SendHARTCommand(cmd);
                }
            });
        }

        private void btn_HART_trim4mA_Click(object sender, EventArgs e)
        {
            var currReading = (float)currentStabilityCalc.MeanValue;
            var cmd = new HART_Trim_4mA_Command(currReading);
            Task.Run(() =>
            {
                lock(hart_communicator)
                {
                    SendHARTCommand(cmd);
                }
            });
        }

        private void btn_HART_trim20mA_Click(object sender, EventArgs e)
        {
            var currReading = (float)currentStabilityCalc.MeanValue;
            var cmd = new HART_Trim_20mA_Command(currReading);
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    SendHARTCommand(cmd);
                }
            });
        }

        private CommandResult SendHARTCommand(HARTCommand cmd)
        {
            CommandResult commres = null;
            HART_SEND_ZERO_COMMAND();
            if (HartAddr is LongAddress)
            {
                commres = hart_communicator.Send(20, HartAddr, cmd);

                if (commres == null) HART_DISCONNECT();
            }
            return commres;
        }

        private void HART_SEND_ZERO_COMMAND()
        {
            if (HartAddr is ShortAddress && ((ShortAddress)HartAddr).PollingAddress == 0)
            {

                var zeroCommandRes = hart_communicator.SendZeroCommand();
                if (zeroCommandRes != null)
                {
                    var addr = new LongAddress(zeroCommandRes.Data[1], zeroCommandRes.Data[2], new byte[] { zeroCommandRes.Data[9], zeroCommandRes.Data[10], zeroCommandRes.Data[11] });
                    HartAddr = addr;
                }
            }

        }
        private void HART_READ_PV_mA_SV_TV_QV()
        {
            var cmd = new HARTCommand(3, new byte[0]);
            var commres = SendHARTCommand(cmd);
            // PRIMRY variable
            if (commres != null)
            {

                if (commres.Data.Length >= 9)
                {
                    var pv = BitConverter.ToSingle(new byte[] { commres.Data[8], commres.Data[7], commres.Data[6], commres.Data[5] }, 0);
                    var mA = BitConverter.ToSingle(new byte[] { commres.Data[3], commres.Data[2], commres.Data[1], commres.Data[0] }, 0);
                    InvokeControlAction( () =>
                    {
                        tb_HART_PV.Text = pv.ToString("N4");
                        tb_HART_PV_MA.Text = mA.ToString("N4");
                    });
                }
                // SECONDARY variable
                if (commres != null && commres.Data.Length >= 14)
                {
                    var sv = BitConverter.ToSingle(new byte[] { commres.Data[13], commres.Data[12], commres.Data[11], commres.Data[10] }, 0);
                    InvokeControlAction( () =>
                    {
                        tb_HART_SV.Text = sv.ToString("N4");
                    });
                }
                // TERTIARY variable
                if (commres != null && commres.Data.Length >= 19)
                {
                    var tv = BitConverter.ToSingle(new byte[] { commres.Data[18], commres.Data[17], commres.Data[16], commres.Data[15] }, 0);
                    InvokeControlAction( () =>
                    {
                        tb_HART_TV.Text = tv.ToString("N4");
                    });
                }
                // QUATERNARY variable
                if (commres != null && commres.Data.Length == 24)
                {
                    var qv = BitConverter.ToSingle(new byte[] { commres.Data[23], commres.Data[22], commres.Data[21], commres.Data[20] }, 0);
                    InvokeControlAction( () =>
                    {
                        tb_HART_QV.Text = qv.ToString("N4");
                    });
                }
            }
            else
            {
                HART_DISCONNECT();
            }
        }

        private void HART_READ_TAG()
        {
            var cmd = new HARTCommand(13, new byte[0]);
            var commres = SendHARTCommand(cmd);
            if (commres != null && commres.Data.Length == 21)
            {
                var s1 = HART_unpack_3bytes_to_string(new byte[] { commres.Data[0], commres.Data[1], commres.Data[2] });
                var s2 = HART_unpack_3bytes_to_string(new byte[] { commres.Data[3], commres.Data[4], commres.Data[5] });
                var tag = $"{s1}{s2}";
                InvokeControlAction( () => { tb_HART_TAG.Text = tag; });
            }
            else
            {
                HART_DISCONNECT();
            }
        }
        private void HART_DISCONNECT()
        {
            HartAddr = new ShortAddress(0);
            InvokeControlAction( () =>
            {
                tb_HART_PV.Text = string.Empty;
                tb_HART_PV_MA.Text = string.Empty;
                tb_HART_TAG.Text = "нет связи.";
            });
        }
        private string HART_unpack_3bytes_to_string(byte[] bytes)
        {
            // 11111122 22223333 33444444
            if (bytes.Length != 3) throw new ArgumentException();
            var c1 = (bytes[0] & 0b11111100 ) >> 2;
            var c2 = (bytes[0] & 0b00000011) << 4 | (bytes[1] & 0b11110000) >> 4;
            var c3 = (bytes[1] & 0b00001111) << 2 | (bytes[2] & 0b11000000) >> 6;
            var c4 = (bytes[2] & 0b00111111);

            c1 |= ((c1 & 0b00100000) ^ 0b00100000) << 1;
            c2 |= ((c2 & 0b00100000) ^ 0b00100000) << 1;
            c3 |= ((c3 & 0b00100000) ^ 0b00100000) << 1;
            c4 |= ((c4 & 0b00100000) ^ 0b00100000) << 1;

            
            var result = System.Text.Encoding.ASCII.GetString(new byte[] { (byte)c1, (byte)c2, (byte)c3, (byte)c4 });
            return result;
        }

        private void HART_READ_SCALE()
        {
            HART_SEND_ZERO_COMMAND();
            if (HartAddr is LongAddress)
            {
                var commres = hart_communicator.Send(20, HartAddr, new HARTCommand(15, new byte[0]));
                var scaleMax = BitConverter.ToSingle(new byte[] { commres.Data[6], commres.Data[5], commres.Data[4], commres.Data[3] }, 0);
                var scaleMin = BitConverter.ToSingle(new byte[] { commres.Data[10], commres.Data[9], commres.Data[8], commres.Data[7] }, 0);
                var step = (scaleMax - scaleMin) / 4;
                InvokeControlAction( () =>
                {
                    tbScaleMax.Text = scaleMax.ToString("0.0000");
                    tbScaleMin.Text = scaleMin.ToString("0.0000");
                    tb_cpcStep.Text = step.ToString("0.0000");
                    tb_PressureSetPoint.Text = scaleMin.ToString("0.0000");
                });
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
}
