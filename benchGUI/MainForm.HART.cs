using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using m4dHART._2_DataLinkLayer;
using m4dHART._2_DataLinkLayer.Wired_Token_Passing;
using m4dHART._7_ApplicationLayer.CommandResponses;
using m4dHART._7_ApplicationLayer.Commands;
using m4dHART._7_ApplicationLayer.CommonTables;
using WorkBench;
using WorkBench.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace benchGUI
{
    public partial class MainForm
    {
        IAddress HartAddr { get; set; } = new ShortAddress(0);
        private TokenPassingDataLinkLayer hart_communicator { get; set; }
        Task HartBackgroundWorker { get; set; }
        CancellationTokenSource HartBackgroundWorkerCTS { get; set; }
        CancellationToken HartBackgroundWorkerCT { get; set; }

        CommandResponseBase lastReceivedCommandResult;
        DateTime lastReceivedCommandResultDateTime;
        DateTime LastPingTime = DateTime.Now;

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

                hart_communicator = new TokenPassingDataLinkLayer(selectedHartPortName);
                var openRes = hart_communicator.Open();
                switch (openRes)
                {
                    case OpenResult.Success:
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
                    default:
                        hart_communicator.Close();
                        break;
                }
            }

        }

        private void HARTCommunicator_Close()
        {
            HART_DISCONNECT();
            if (hart_communicator == null) return;

            hart_communicator.BACKReceived -= OnBACKReceived;
            hart_communicator.Close();
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

            tb_longTag.Text = string.Empty;
            tb_HART_PV.Text = string.Empty;
            tb_HART_SV.Text = string.Empty;
            tb_HART_TV.Text = string.Empty;
            tb_HART_QV.Text = string.Empty;
            tb_HART_PV_MA.Text = string.Empty;
            tb_HART_Damping.Text = string.Empty;
            cb_HART_Xfer_Function.DataSource = null;

            tb_HART_PV.BackColor = DefaultBackColor;
            btn_HART_BURST_OFF.BackColor = DefaultBackColor;

            gb_HART.BackColor = DefaultBackColor;
            tb_HART_PV_MA.BackColor = DefaultBackColor;
        }

        private void OnBACKReceived(CommandResponseBase commres)
        {
            lastReceivedCommandResultDateTime = DateTime.Now;
            if ( commres is CommandResponse cr && cr.CommandNumber == 3)
            {
                Process_PV_mA_SV_TV_QV(cr);
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
                        if (!HartBackgroundWorkerCT.IsCancellationRequested && !lastReceivedCommandResult.BURST )
                                HART_READ_PV_mA_SV_TV_QV();
                        }
                        if (DateTime.Now  - lastReceivedCommandResultDateTime > TimeSpan.FromSeconds(5))
                        {
                            HART_DISCONNECT();
                        }
                        if (DateTime.Now - LastPingTime > TimeSpan.FromMilliseconds(1500))
                        {
                            HART_READ_PV_mA_SV_TV_QV();
                            LastPingTime = DateTime.Now;

                        }
                    }
                }
            });
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
            var cmd = new HART_109_Burst_Mode_Control( _009_Burst_Mode_Control_Code.Off, 0);
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    var cmdResult = SendHARTCommand(cmd);
                    var fieldDevStatus = cmdResult.DeviceStatus;
                }
            });
        }
        private void btn_HART_BURST_ON_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    HARTCommand cmd = new HARTCommand(108, new byte[] { 3 });
                    var cmdResult = SendHARTCommand(cmd);
                    cmd = new HART_109_Burst_Mode_Control(_009_Burst_Mode_Control_Code.Enable_on_Token_Passing_Data_Link_Layer_only, 3);
                    cmdResult = SendHARTCommand(cmd);

                }
            });
        }

        private void btn_HART_set_4mA_Click(object sender, EventArgs e)
        {
            SetHart_mA_level(4f).ContinueWith((t) =>
            {
                var commandResult = t.Result;
                if (commandResult is CommandResponse && commandResult.CommandNumber == 40 && commandResult.DeviceStatus.LoopCurrentFixed)
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
                if (commandResult is CommandResponse && commandResult.CommandNumber == 40 && commandResult.DeviceStatus.LoopCurrentFixed)
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
                if (commandResult is CommandResponse && commandResult.CommandNumber == 40 && !commandResult.DeviceStatus.LoopCurrentFixed)
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

        private Task<CommandResponseBase> SetHart_mA_level(Single mAlevel)
        {
            var cmd = new HART_040_Simulate_Current_Command(mAlevel);
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
            float currReading;
            if (currentReaderSpan == null)
            {
                using (var currInputDialogue = new GetMeasuredCurrentValueDialog())
                {
                    currInputDialogue.StartPosition = FormStartPosition.CenterParent;
                    var dialogueResult = currInputDialogue.ShowDialog();
                    if (dialogueResult == DialogResult.OK)
                    {
                        currReading = currInputDialogue.MeasuredCurrentValue;
                    }
                    else
                    {
                        return;
                    }
                }

            }
            else
            {
                currReading = (float)currentStabilityCalc.MeanValue;
            }
            var cmd = new HART_045_Trim_Loop_Current_Zero(currReading);
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
            float currReading;
            if (currentReaderSpan == null)
            {
                using (var currInputDialogue = new GetMeasuredCurrentValueDialog())
                {
                    currInputDialogue.StartPosition = FormStartPosition.CenterParent;
                    var dialogueResult = currInputDialogue.ShowDialog();
                    if (dialogueResult == DialogResult.OK)
                    {
                        currReading = currInputDialogue.MeasuredCurrentValue;
                    }
                    else
                    {
                        return;
                    }
                }

            }
            else
            {
                currReading = (float)currentStabilityCalc.MeanValue;
            }
            var cmd = new HART_046_Trim_Trim_Loop_Current_Gain(currReading);
            Task.Run(() =>
            {
                lock (hart_communicator)
                {
                    SendHARTCommand(cmd);
                }
            });
        }

        private CommandResponseBase SendHARTCommand(HARTCommand cmd)
        {
            CommandResponseBase commres = null;
            HART_SEND_ZERO_COMMAND();
            if (HartAddr is LongAddress)
            {
                commres = hart_communicator.Send(5, HartAddr, cmd);

                if (commres == null) HART_DISCONNECT();
                else
                {
                    lastReceivedCommandResult = commres;
                    lastReceivedCommandResultDateTime = DateTime.Now;
                    InvokeControlAction(() =>
                    {
                        if (commres.BURST)
                        {
                            btn_HART_BURST_OFF.BackColor = System.Drawing.Color.LightGreen;
                        }
                        else
                        {
                            btn_HART_BURST_OFF.BackColor = Control.DefaultBackColor;
                        }
                        // !!!!!!!! DEVICE MALFUNCTION !!!!!!!!
                        if (commres.DeviceStatus.DeviceMalfunction)
                        {
                            gb_HART.BackColor = Color.Red;
                        }
                        else
                        {
                            gb_HART.BackColor = DefaultBackColor;
                        }
                        // LOOP Current Levels
                        if (commres.DeviceStatus.LoopCurrentFixed)
                        {
                            tb_HART_PV_MA.BackColor = Color.LightYellow;
                        }
                        else if (commres.DeviceStatus.LoopCurrentSaturated)
                        {
                            tb_HART_PV_MA.BackColor = Color.OrangeRed;
                        }
                        else
                        {
                            tb_HART_PV_MA.BackColor = DefaultBackColor;

                        }
                    });

                    if (commres is CommandResponseCommError)
                    {
                        log4net.LogManager.GetLogger("HART").Debug($"{commres}");
                        return SendHARTCommand(cmd);
                    }
                    if (commres is CommandResponseMasterCommandError commerr && commerr.ResponseCode.Busy)
                    {
                        return SendHARTCommand(cmd);
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
                var dynVarAssignments = SendHARTCommand(new HART_050_Read_Dynamic_Variables_Assignments());
                if (dynVarAssignments is HART_Result_050_Read_Dynamic_Variables_Assignments dynResp)
                {
                    var PVVarInfo = SendHARTCommand(new HART_054_Read_Device_Variable_Information(dynResp.DeviceVariable_assigned_to_PV));
                    if (PVVarInfo is HART_Result_054_Read_Device_Variable_Information_cs pvinfo)
                    {
                        InvokeControlAction(() =>{
                        System.Windows.Forms.ToolTip tt = new System.Windows.Forms.ToolTip();
                        tt.InitialDelay = 1;
                        tt.ShowAlways = true;
                        tt.SetToolTip(tbScaleMin, $"{pvinfo.LowerTransducerLimit}");
                        
                        tt = new System.Windows.Forms.ToolTip();
                        tt.InitialDelay = 1;
                        tt.ShowAlways = true;
                        tt.SetToolTip(tbScaleMax, $"{pvinfo.UpperTransducerLimit}");
                        });
                    }
                    else
                    {
                        InvokeControlAction(() =>{
                        System.Windows.Forms.ToolTip tt = new System.Windows.Forms.ToolTip();
                        tt.SetToolTip(tbScaleMin, $"NA");
                        tt.SetToolTip(tbScaleMax, $"NA");
                        });
                    }
                }
                else
                {
                    InvokeControlAction(() => {
                        System.Windows.Forms.ToolTip tt = new System.Windows.Forms.ToolTip();
                        tt.SetToolTip(tbScaleMin, $"NA");
                        tt.SetToolTip(tbScaleMax, $"NA");
                    });
                }
            }
        }
        private void HART_READ_PV_mA_SV_TV_QV()
        {
            var cmd = new HARTCommand(3, new byte[0]);
            var commres = SendHARTCommand(cmd);
            // PRIMRY variable
            if (commres is CommandResponse cr && cr.CommandNumber == 3)
            {
                Process_PV_mA_SV_TV_QV(cr);
            }
        }
        private void Process_PV_mA_SV_TV_QV(CommandResponse commres)
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
            var cmd = new HART_013_Read_Tag_Descriptor_Date();
            var commres = SendHARTCommand(cmd);
            if (commres is HART_Result_013_Tag_Descriptor_Date aaa)
            {
                InvokeControlAction( () => { tb_HART_TAG.Text = aaa.Tag; });
            }
        }
        private void HART_READ_LONGTAG()
        {
            var cmd = new HART_020_ReadLongTag();
            var commres = SendHARTCommand(cmd);
            //if (commres != null && commres.Data.Length == 32)
            //{
            //    var longtag = Encoding.Latin1.GetString(commres.Data);
            //    InvokeControlAction(() => { tb_longTag.Text = longtag; });
            //}
            InvokeControlAction(() =>
            {
                if (commres is HART_Result_020_Read_Long_Tag res)
                {
                    tb_longTag.Enabled = true;
                    tb_longTag.Text = res.LongTag;
                }
                if (commres is CommandResponseMasterCommandError me && me.ResponseCode.NotImplemented)
                {
                    tb_longTag.Text = string.Empty;
                    tb_longTag.Enabled = false;

                }
            });
        }


        private void HART_DISCONNECT()
        {
            HartAddr = new ShortAddress(0);
            InvokeControlAction( () =>
            {
                if (hart_communicator != null)
                {
                    hart_communicator.BACKReceived -= OnBACKReceived;
                }

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
                        var commres = SendHARTCommand(new HART_035_Write_PrimaryVariableRangeValues(uomCode, tbScaleMin.Text.ParseToDouble(), tbScaleMax.Text.ParseToDouble()));
                        if (commres is HART_Result_035_Write_PrimaryVariableRangeValues res)
                        {
                            commres = SendHARTCommand(new HART_044_Write_Primary_Variable_Units(uomCode));
                        }
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
                var commres = SendHARTCommand(new HART_015_Read_Device_Information());
                if (commres is HART_Result_015_Read_Device_Information res && (commres is CommandResponse cr) && cr.ResponseCode.Success) 
                {

                    var scaleMax = res.PVUpperRangeValue;
                    var scaleMin = res.PVLowerRangeValue;
                    var step = (scaleMax - scaleMin) / 4;
                    InvokeControlAction(() =>
                    {
                        tbScaleMax.Text = scaleMax.ToString("0.0000");
                        tbScaleMin.Text = scaleMin.ToString("0.0000");
                        tb_cpcStep.Text = step.ToString("0.0000");
                        tb_PressureSetPoint.Text = scaleMin.ToString("0.0000");

                        tb_HART_Damping.Text = res.PVDampingValue_in_seconds.ToString("0.0");

                        cb_HART_Xfer_Function.DataSource = Enum.GetValues(typeof(_003_Transfer_Function_Code));

                        cb_HART_Xfer_Function.SelectedItem = res.PVTransferFunctionCode;
                    });
                    switch (res.PVUpperAndLowerRangeValuesUnitCode)
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
                        HARTCommand cmd = new HART_013_Read_Tag_Descriptor_Date();
                        var commres = SendHARTCommand(cmd);
                        if (commres is HART_Result_013_Tag_Descriptor_Date aaa)
                        {
                            cmd = new HART_018_Write_Tag_Descriptor_Date(tb_HART_TAG.Text, aaa.Descriptor, DateTime.Now);
                            commres = SendHARTCommand(cmd);
                        }
                        return commres;
                    }
                }).ContinueWith((a) =>
                {
                    if (a.Result is HART_Result_018_Write_Tag_Descriptor_Date result)
                    {
                        InvokeControlAction(() => tb_HART_TAG.Text = result.Tag); 
                    }
                    if (a.Result is HART_Result_013_Tag_Descriptor_Date result1)
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
                        var cmd = new HART_022_Write_Long_Tag(tb_longTag.Text);
                        var commres = SendHARTCommand(cmd);
                        return commres;
                    }
                }).ContinueWith((a) =>
                {
                    if (a.Result is HART_Result_022_Write_Long_Tag result)
                    {
                        InvokeControlAction(() => tb_longTag.Text = result.LongTag);
                    }
                });
            }
        }

        private void tb_HART_Damping_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && hart_communicator != null)
            {
                if (tb_HART_Damping.Text.TryParseToDouble(out double newDampingValue))
                {

                    Task.Run(() =>
                    {
                        lock (hart_communicator)
                        {
                            var cmd = new HART_034_Write_Primary_Variable_Damping_Value((float)newDampingValue);
                            var commres = SendHARTCommand(cmd);
                            return commres;
                        }
                    }).ContinueWith((a) =>
                    {
                        if (a.Result is HART_Result_034_Write_Primary_Variable_Damping_Value result)
                        {
                            InvokeControlAction(() => tb_HART_Damping.Text = result.ActualPVDampingValue.ToString("0.0"));
                        }
                        else
                        {
                            lock (hart_communicator)
                            {
                                HART_READ_SCALE();
                            }
                        }
                    });
                }
                else
                {
                    Task.Run(() =>
                    {
                        lock (hart_communicator)
                        {
                            HART_READ_SCALE();
                        }
                    });
                }
            }
        }
        private void cb_HART_Xfer_Function_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (hart_communicator != null && Enum.TryParse(cb_HART_Xfer_Function.SelectedValue.ToString(), out _003_Transfer_Function_Code newXferCode))
            {
                Task.Run(() =>
                {
                    lock (hart_communicator)
                    {
                        var cmd = new HART_047_Write_PrimaryVariable_Transfer_Function(newXferCode);
                        var commres = SendHARTCommand(cmd);
                        return commres;
                    }
                }).ContinueWith((a) =>
                {
                    if (a.Result is HART_Result_047_Write_PrimaryVariable_Transfer_Function result)
                    {
                        InvokeControlAction(() => cb_HART_Xfer_Function.SelectedItem = result.PVTransferFunctionCode);
                    }
                    else
                    {
                        lock (hart_communicator)
                        {
                            HART_READ_SCALE();
                        }
                    }
                });
            }
        }
    }
}
