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
using WorkBench.UOM;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    partial class ElmetroPascal
    {
        #region Elmetro Pascal hardware interface commands
        //------------------------------------------------------------------------------------------

        bool SwitchToREMOTEMode ()
        {
            var res = _communicator.QueryCommand("R");
            if (res.Contains("REMOTE"))
            {
                return true;
            }
            return false;

        }

        bool SwitchToLOCALMode()
        {
            var res = _communicator.QueryCommand("LOCAL");
            if (res.Contains("OK"))
            {
                return true;
            }
            return false;
        }

        Collection<ElmetroPascalScale> ReadInternalModuleRanges()
        {
            return ParseModuleScales( _communicator.QueryCommand("READ_M1?"), 1);
        }

        Collection<ElmetroPascalScale> ReadExternalModuleRanges()
        {
            _communicator.QueryCommand("SEEK_MODUL");
            Thread.Sleep(1000);
            return ParseModuleScales(_communicator.QueryCommand("READ_M2?"), 2);
        }

        Collection<ElmetroPascalScale> UsableRanges()
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

        internal bool StartPressureGeneration ()
        {
            var res = _communicator.QueryCommand("ON_KEY_START");
            if (res.Contains("START_REGULATION"))
            {
                return true;
            }
            res = _communicator.QueryCommand("ON_KEY_START");
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
            var res = _communicator.QueryCommand("ON_KEY_START");
            if (res.Contains("STOP_REGULATION"))
            {
                return true;
            }
            res = _communicator.QueryCommand("ON_KEY_START");
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
            var res = _communicator.QueryCommand("ON_KEY_VENT");
            if (res.Contains("VENT_ON"))
            {
                return true;
            }
            res = _communicator.QueryCommand("ON_KEY_VENT");
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
            var res = _communicator.QueryCommand("ON_KEY_VENT");
            if (res.Contains("VENT_OFF"))
            {
                return true;
            }
            res = _communicator.QueryCommand("ON_KEY_VENT");
            if (res.Contains("VENT_OFF"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        internal bool SetSetPoint(double SetPointIn_kPa)
        {
            var res = _communicator.QueryCommand(string.Format("TARGET {0}", SetPointIn_kPa.ToString(CultureInfo.InvariantCulture)));
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
            string pattern = @"([0-9]{1})[:]{1}\s*(?:\[\s*(\d*\.*\d)*\s*(\d*\.*\d*)\])*";
            var matches = Regex.Matches(moduleResponse, pattern, RegexOptions.IgnoreCase);
            Collection<ElmetroPascalScale> scales = new Collection<ElmetroPascalScale>();
            if (matches.Count == 1 && matches[0].Success)
            {
                if (!int.TryParse(matches[0].Groups[1].Value, out int rangesCount)) throw new Exception(string.Format("invalid module ranges response {0}", moduleResponse));
                for (int i = 0; i < rangesCount; i++)
                {
                    scales.Add(new ElmetroPascalScale()
                    {
                        Min = float.Parse(matches[0].Groups[2].Captures[i].Value, new System.Globalization.NumberFormatInfo()),
                        Max = float.Parse(matches[0].Groups[3].Captures[i].Value, new System.Globalization.NumberFormatInfo()),
                        UOM = new kPa(),
                        Module = moduleNumber,
                        RangeNum = i + 1
                    });
                }
            }

            return scales;

        }

        //------------------------------------------------------------------------------------------
        #endregion
    }
}
