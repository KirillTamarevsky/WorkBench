using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    partial class ElmetroPascal
    {
        #region Elmetro Pascal hardware interface commands
        //------------------------------------------------------------------------------------------

        bool SwitchToREMOTEMode ()
        {
            var res = Communicator.QueryCommand("R");
            if (!string.IsNullOrEmpty(res) && res.Contains("REMOTE"))
            {
                _in_REMOTE_mode = true;

                return true;
            }
            return false;

        }

        bool SwitchToLOCALMode()
        {
            var res = Communicator.QueryCommand("LOCAL");
            if (!string.IsNullOrEmpty(res) && res.Contains("OK"))
            {
                _in_REMOTE_mode = false;
                return true;
            }
            return false;
        }
        //RANGE n,m
        //n указывает номер модуля (1 - внутренний модуль, 2 - внешний модуль)
        //m – порядковый номер устанавливаемого диапазона(определяется по списку диапазонов из команд READ_M1?, READ_M2?)
        internal bool SetActiveModule_Range(ElmetroPascalChannelSpan epspan)
        {
            var scale = epspan.Scale as ElmetroPascalScale;
            if (scale != null)
            {
                var reply = Communicator.QueryCommand($"RANGE {scale.Module},{scale.RangeNum}");
                if (reply.Contains("OK"))
                {
                    return true;
                }
            }
            return false;
        }
        //CLEAR_P
        internal bool Zeroing()
        {
            var reply = Communicator.QueryCommand(string.Format("CLEAR_P"));
            if (reply.Contains("OK"))
            {
                return true;
            }
            return false;
        }
        Collection<ElmetroPascalScale> ReadInternalModuleRanges()
        {
            return ParseModuleScales(Communicator.QueryCommand("READ_M1?"), 1);
        }

        Collection<ElmetroPascalScale> ReadExternalModuleRanges()
        {
            Communicator.QueryCommand("SEEK_MODUL");
            Thread.Sleep(1000);
            return ParseModuleScales(Communicator.QueryCommand("READ_M2?"), 2);
        }

        internal Collection<ElmetroPascalScale> UsableRangesAsync()
        {
            var internalModuleRanges = ReadInternalModuleRanges();
            var externalModuleRanges = ReadExternalModuleRanges();
            if (externalModuleRanges.Count > 0)
            {
                var internalModuleMaxPressure = internalModuleRanges.OrderByDescending(s => s.Max).FirstOrDefault().Max;
                var externalModuleMaxPressure = externalModuleRanges.OrderByDescending(s => s.Max).FirstOrDefault().Max;
            
                if (externalModuleMaxPressure > internalModuleMaxPressure)
                {
                    externalModuleRanges = new Collection<ElmetroPascalScale>( externalModuleRanges.Where(r => r.Max <= internalModuleMaxPressure).ToList());
                }

                if (internalModuleMaxPressure > externalModuleMaxPressure)
                {
                    internalModuleRanges = new Collection<ElmetroPascalScale>(internalModuleRanges.Where(r => r.Max <= externalModuleMaxPressure).ToList());
                }

                return new Collection<ElmetroPascalScale>( internalModuleRanges.Union(externalModuleRanges).OrderBy(s => s.Max).ToArray() );
            }

            return internalModuleRanges;
        }

        internal OneMeasure GetActualPressure()
        {
            var res = Communicator.QueryCommand("PRES?").Trim().Replace(',','.');

            if( float.TryParse(res, NumberStyles.Float, new System.Globalization.NumberFormatInfo(), out float pressureValue)) 
                return new OneMeasure(pressureValue, new kPa());

            return null;
        }

        internal bool StartPressureGeneration ()
        {
            var res = Communicator.QueryCommand("ON_KEY_START");
            if (res.Contains("START_REGULATION"))
            {
                return true;
            }
            res = Communicator.QueryCommand("ON_KEY_START");
            if (res.Contains("START_REGULATION"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool StopPressureGeneration ()
        {
            var res = Communicator.QueryCommand("ON_KEY_START");
            if (res.Contains("STOP_REGULATION"))
            {
                return true;
            }
            res = Communicator.QueryCommand("ON_KEY_START");
            if (res.Contains("STOP_REGULATION"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool VentOpen()
        {
            var res = Communicator.QueryCommand("ON_KEY_VENT");
            if (res.Contains("VENT_ON"))
            {
                return true;
            }
            res = Communicator.QueryCommand("ON_KEY_VENT");
            if (res.Contains("VENT_ON"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal bool VentClose()
        {
            var res = Communicator.QueryCommand("ON_KEY_VENT");
            if (res.Contains("VENT_OFF"))
            {
                return true;
            }
            res = Communicator.QueryCommand("ON_KEY_VENT");
            if (res.Contains("VENT_OFF"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// устанавливает целевое значение давления в кПа
        /// </summary>
        /// <param name="SetPointIn_kPa"></param>
        /// <returns></returns>
        internal bool SetSetPoint(double SetPointIn_kPa)
        {
            var res = Communicator.QueryCommand($"TARGET {SetPointIn_kPa.ToString(CultureInfo.InvariantCulture)}");
            if (res.Contains("OK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static Collection<ElmetroPascalScale> ParseModuleScales(string moduleResponse, int moduleNumber)
        {
            // "2: [   0.0 3500.0][   0.0 1600.0]"
            // "4: [   0.0 6000.0][   0.0 4000.0][   0.0 2500.0][   0.0 1600.0]"
            // ([0-9]{1})[:]{1}\s*(\[\s*([0-9]*\.*[0-9])*\s*([0-9]*\.*[0-9]*)\])*
            string pattern = @"([0-9]{1})[:]{1}\s*(?:\[\s*(\d*\.*\d)*\s*(\d*\.*\d*)\]\s*)*";
            var matches = Regex.Matches(moduleResponse, pattern, RegexOptions.IgnoreCase);
            Collection<ElmetroPascalScale> scales = new Collection<ElmetroPascalScale>();
            if (matches.Count == 1 && matches[0].Success)
            {
                int rangesCount;
                if (!int.TryParse(matches[0].Groups[1].Value, out rangesCount)) throw new Exception(string.Format("invalid module ranges response {0}", moduleResponse));
                for (int i = 0; i < rangesCount; i++)
                {
                    scales.Add(new ElmetroPascalScale(
                        float.Parse(matches[0].Groups[2].Captures[i].Value, new System.Globalization.NumberFormatInfo()),
                        float.Parse(matches[0].Groups[3].Captures[i].Value, new System.Globalization.NumberFormatInfo()),
                        new kPa()
                        )
                    {
                        Module = moduleNumber,
                        RangeNum = i + 1
                    }
                    );
                }
            }

            return scales;

        }

        //------------------------------------------------------------------------------------------
        #endregion
    }
}
