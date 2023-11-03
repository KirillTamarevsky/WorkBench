using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication.HartLite;
using Communication.HartLite.CommandResults;
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

        CommandResult lastReceivedCommandResult;
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
                        btn_HART_BURST_OFF.Enabled = true;
                        btn_HART_BURST_ON.Enabled = true;

                        hart_communicator.BACKReceived += OnBACKReceived;
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

        private void OnBACKReceived(CommandResult commres)
        {
            if ( !commres.HasCommunicationError && commres.CommandNumber == 3)
            {
                Process_PV_mA_SV_TV_QV(commres);
                //if (btn_HART_BURST_OFF.BackColor != Control.DefaultBackColor) { btn_HART_BURST_OFF.BackColor = Control.DefaultBackColor; }
                //else
                //{
                //    btn_HART_BURST_OFF.BackColor = Color.LightGreen;
                //}
            }
        }

        private Task HartBackgroundWorker_DoWork()
        {
            return Task.Run(() =>
            {
                while (!HartBackgroundWorkerCT.IsCancellationRequested)
                {
                    Thread.Sleep(50);
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
                                {
                                    HART_READ_TAG();
                                    HART_READ_LONGTAG();
                                }

                            }
                        }
                        if (HartAddr is LongAddress)
                        {
                        if (!HartBackgroundWorkerCT.IsCancellationRequested && !lastReceivedCommandResult.Address._fieldDeviceInBurstMode && !(lastReceivedCommandResult.FrameType == m4dHART._2_DataLinkLayer.Wired_Token_Passing.FrameType.BACK ))
                                HART_READ_PV_mA_SV_TV_QV();
                            
                        }
                    }
                }
            });
        }

        private void HARTCommunicator_Close()
        {
            hart_communicator.BACKReceived -= OnBACKReceived;
            var closeRes = hart_communicator.Close();
            HartAddr = new ShortAddress(0);
            if (HartBackgroundWorker != null)
            {
                HartBackgroundWorkerCTS.Cancel();
                HartBackgroundWorker.Wait();
                HartBackgroundWorkerCTS.Dispose();
                HartBackgroundWorkerCTS = null;
                HartBackgroundWorker.Dispose();
                HartBackgroundWorker = null;
            }
            hart_communicator = null;
            btn_HART_open.Text = "Открыть";
            btn_HART_ZEROTRIM.Enabled = false;
            btn_ReadHART_Scale.Enabled = false;
            btn_HART_set_0mA.Enabled = false;
            btn_HART_set_4mA.Enabled = false;
            btn_HART_set_20mA.Enabled = false;
            btn_HART_trim4mA.Enabled = false;
            btn_HART_trim20mA.Enabled = false;
            btn_HART_BURST_OFF.Enabled = false;
            btn_HART_BURST_ON.Enabled = false;
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


        private void btn_HART_BURST_OFF_Click(object sender, EventArgs e)
        {
            var cmd = new HART_109_Burst_Mode_Control( Communication.HartLite.CommonTables._009_Burst_Mode_Control_Code.Off, 0);
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    var cmdResult = SendHARTCommand(cmd);
                    var commStat = cmdResult.CommandStatus.FirstByte;
                    var fieldDevStatus = cmdResult.CommandStatus.FieldDeviceStatus;
                }
            });
        }
        private void btn_HART_BURST_ON_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    var cmd = new HARTCommand(108, new byte[] { 3 });
                    var cmdResult = SendHARTCommand(cmd);
                    cmd = new HART_109_Burst_Mode_Control(Communication.HartLite.CommonTables._009_Burst_Mode_Control_Code.Enable_on_Token_Passing_Data_Link_Layer_only, 3);
                    cmdResult = SendHARTCommand(cmd);

                }
            });
        }

        private void btn_HART_set_4mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(4f).ContinueWith((t) =>
            {
                var commandResult = t.Result;
                if (commandResult != null && commandResult.CommandNumber == 40 && commandResult.CommandStatus.FieldDeviceStatus.LoopCurrentFixed)
                {
                    InvokeControlAction( () =>
                    {
                        btn_HART_trim4mA.Enabled = true;
                        btn_HART_trim20mA.Enabled = false;
                    });

                }
            });
        }
        private void btn_HART_set_20mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(20f).ContinueWith((t) =>
            {
                var commandResult = t.Result;
                if (commandResult != null && commandResult.CommandNumber == 40 && commandResult.CommandStatus.FieldDeviceStatus.LoopCurrentFixed)
                {
                    InvokeControlAction( () =>
                    {
                        btn_HART_trim4mA.Enabled = false;
                        btn_HART_trim20mA.Enabled = true;
                    });
                }
            });
        }
        private void btn_HART_set_0mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(0f).ContinueWith((t) =>
            {
                var commandResult = t.Result;
                if (commandResult != null && commandResult.CommandNumber == 40 && !commandResult.CommandStatus.FieldDeviceStatus.LoopCurrentFixed)
                {
                    InvokeControlAction( () =>
                    {
                        btn_HART_trim4mA.Enabled = false;
                        btn_HART_trim20mA.Enabled = false;
                    });
                }
            }
                );
        }

        private Task<CommandResult> SetHart_mA_level(Single mAlevel)
        {
            var cmd = new HART_40_Simulate_Current_Command(mAlevel);
            return Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    var commres = SendHARTCommand(cmd);
                    return commres;
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
                commres = hart_communicator.Send(5, HartAddr, cmd);

                if (commres == null) HART_DISCONNECT();
                else
                {
                    lastReceivedCommandResult = commres;
                    if (commres.HasCommunicationError)
                    {
                        return SendHARTCommand(cmd);
                    }
                    if (commres.Address._fieldDeviceInBurstMode)
                    {
                        btn_HART_BURST_OFF.BackColor = System.Drawing.Color.LightGreen;
                    }
                    else
                    {
                        btn_HART_BURST_OFF.BackColor = Control.DefaultBackColor;
                    }
                }
            }
            return commres;
        }

        private void HART_SEND_ZERO_COMMAND()
        {
            if (HartAddr is ShortAddress && ((ShortAddress)HartAddr).PollingAddress == 0)
            {

                var zeroCommandRes = hart_communicator.SendZeroCommand();
                if (zeroCommandRes is IReadUniqueIdetifierCommand uniqueIDcommand )
                {
                    HartAddr = uniqueIDcommand.LongAddress;
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
                Process_PV_mA_SV_TV_QV(commres);
            }
        }
        private void Process_PV_mA_SV_TV_QV(CommandResult commres)
        {
            InvokeControlAction(() =>
            {
                if (commres.Data.Length >= 9)
                {
                    var pv = BitConverter.ToSingle(new byte[] { commres.Data[8], commres.Data[7], commres.Data[6], commres.Data[5] }, 0);
                    var mA = BitConverter.ToSingle(new byte[] { commres.Data[3], commres.Data[2], commres.Data[1], commres.Data[0] }, 0);
                    InvokeControlAction(() =>
                    {
                        tb_HART_PV.Text = pv.ToString("N4");
                        if (tb_HART_PV.BackColor != Control.DefaultBackColor)
                        {
                            tb_HART_PV.BackColor = Control.DefaultBackColor;
                        }
                        else
                        {
                            tb_HART_PV.BackColor = Color.LightSkyBlue;
                        }
                        tb_HART_PV_MA.Text = mA.ToString("N4");
                    });
                }
                // SECONDARY variable
                if (commres != null && commres.Data.Length >= 14)
                {
                    var sv = BitConverter.ToSingle(new byte[] { commres.Data[13], commres.Data[12], commres.Data[11], commres.Data[10] }, 0);
                    InvokeControlAction(() =>
                    {
                        tb_HART_SV.Text = sv.ToString("N4");
                    });
                }
                // TERTIARY variable
                if (commres != null && commres.Data.Length >= 19)
                {
                    var tv = BitConverter.ToSingle(new byte[] { commres.Data[18], commres.Data[17], commres.Data[16], commres.Data[15] }, 0);
                    InvokeControlAction(() =>
                    {
                        tb_HART_TV.Text = tv.ToString("N4");
                    });
                }
                // QUATERNARY variable
                if (commres != null && commres.Data.Length == 24)
                {
                    var qv = BitConverter.ToSingle(new byte[] { commres.Data[23], commres.Data[22], commres.Data[21], commres.Data[20] }, 0);
                    InvokeControlAction(() =>
                    {
                        tb_HART_QV.Text = qv.ToString("N4");
                    });
                }
            });
        }

        private void HART_READ_TAG()
        {
            var cmd = new HART_13_Read_Tag_Descriptor_Date();
            var commres = SendHARTCommand(cmd);
            if (commres is HART_Result_13_Tag_Descriptor_Date aaa)
            {
                InvokeControlAction( () => { tb_HART_TAG.Text = aaa.Tag; });
            }
        }
        private void HART_READ_LONGTAG()
        {
            var cmd = new HART_20_ReadLongTag();
            var commres = SendHARTCommand(cmd);
            //if (commres != null && commres.Data.Length == 32)
            //{
            //    var longtag = Encoding.Latin1.GetString(commres.Data);
            //    InvokeControlAction(() => { tb_longTag.Text = longtag; });
            //}
            if (commres is HART_Result_20_Read_Long_Tag res)
            {
                InvokeControlAction(() => { tb_longTag.Text = res.LongTag; });

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

        private void tbScaleMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && hart_communicator != null)
            {
                sendNewHARTScale();
            }
        }

        private void sendNewHARTScale()
        {
            if (tbScaleMax.Text.IsFloatString() && tbScaleMin.Text.IsFloatString())
            {
                byte uomCode = 7;
                switch (((IUOM)cbPressureScaleUOM.SelectedItem).Name)
                {
                    case "bar":
                        uomCode = 7; //7 bars
                        break;
                    case "mbar":
                        uomCode = 8; //8 millibars
                        break;
                    case "Pa":
                        uomCode = 11; //11 pascals
                        break;
                    case "kPa":
                        uomCode = 12; //12 kilopascals
                        break;
                    case "MPa":
                        uomCode = 237; //237 megapascals
                        break;

                    case 
                        "°C":
                        uomCode = 32; //32 Degrees Celsius
                        break;
                    case 
                        "°F":
                        uomCode = 33; //33 Degrees Fahrenheit
                        break;
                    case
                        "°R":
                        uomCode = 34; //34 Degrees Rankine
                        break;
                    case "°K":
                        uomCode = 35; //35 Kelvin
                        break;
                    case "mmH2O@4°C":
                        uomCode = 239; //239 millimeters of water at 4 degrees C
                        break;
                    default:
                        break;
                }
                Task.Run(() =>
                {
                    lock (hart_communicator)
                    {
                        SendHARTCommand(new HART_35_Write_PrimaryVariableRangeValues(uomCode, tbScaleMin.Text.ParseToDouble(), tbScaleMax.Text.ParseToDouble()));
                        HART_READ_SCALE();
                    }
                });
            }
            
        }

        private void HART_READ_SCALE()
        {
            HART_SEND_ZERO_COMMAND();
            if (HartAddr is LongAddress)
            {
                var commres = hart_communicator.Send(20, HartAddr, new HARTCommand(15, new byte[0]));
                if (commres != null && !commres.HasCommunicationError)
                {


                    var scaleMax = BitConverter.ToSingle(new byte[] { commres.Data[6], commres.Data[5], commres.Data[4], commres.Data[3] }, 0);
                    var scaleMin = BitConverter.ToSingle(new byte[] { commres.Data[10], commres.Data[9], commres.Data[8], commres.Data[7] }, 0);
                    var step = (scaleMax - scaleMin) / 4;
                    InvokeControlAction(() =>
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
        private void tbShortTag_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && hart_communicator != null)
            {
                Task.Run(() =>
                {
                    lock (hart_communicator)
                    {
                        HARTCommand cmd = new HART_13_Read_Tag_Descriptor_Date();
                        var commres = SendHARTCommand(cmd);
                        if (commres is HART_Result_13_Tag_Descriptor_Date aaa)
                        {
                            cmd = new HART_18_Write_Tag_Descriptor_Date(tb_HART_TAG.Text, aaa.Descriptor, DateTime.Now);
                            commres = SendHARTCommand(cmd);
                        }
                        return commres;
                    }
                }).ContinueWith((a) =>
                {
                    if (a.Result is HART_Result_18_Write_Tag_Descriptor_Date result)
                    {
                        InvokeControlAction(() => tb_HART_TAG.Text = result.Tag); 
                    }
                    if (a.Result is HART_Result_13_Tag_Descriptor_Date result1)
                    {
                        InvokeControlAction(() => tb_HART_TAG.Text = result1.Tag);

                    }
                });
            }
        }
        private void tb_longTag_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && hart_communicator != null)
            {
                Task.Run(() =>
                {
                    lock (hart_communicator)
                    {
                        var cmd = new HART_22_Write_Long_Tag(tb_longTag.Text);
                        var commres = SendHARTCommand(cmd);
                        return commres;
                    }
                }).ContinueWith((a) =>
                {
                    if (a.Result is HART_Result_22_Write_Long_Tag result)
                    {
                        InvokeControlAction(() => tb_longTag.Text = result.LongTag);
                    }
                });
            }
        }
    }
}
