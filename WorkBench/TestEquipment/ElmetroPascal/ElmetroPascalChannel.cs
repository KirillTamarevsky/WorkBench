using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public class ElmetroPascalChannel : IInstrumentChannel
    {
        private object controlPossibleLock = new object();
        internal ElmetroPascal parentEPascal {get;  private set;}

        internal ElmetroPascalChannelSpan ActiveSpan { get; set; }
        internal PressureControllerOperationMode _pressureOperationMode { get ; set; }

        #region IINstrumentChannel
        public int NUM => 1;
        public string Name => "канал давления";
        public IInstrumentChannelSpan[] AvailableSpans { get; private set; }
        #endregion

        public ElmetroPascalChannel(ElmetroPascal elmetroPascal)
        {
            if (elmetroPascal == null) throw new ArgumentNullException($"Элметро-Паскаль = NULL");
            if (!elmetroPascal.IsOpen) throw new ArgumentNullException($"Элметро-Паскаль - нет связи!");

            parentEPascal = elmetroPascal;

            //setup Channel Spans
            var _avSpans = new List<ElmetroPascalChannelSpan>();
            var usableSpans = UsableSpans();
            //иногда некорректно считывается информация о подключенном модуле, верхние и нижние пределы равны 0. 
            //если так, пробуем еще раз
            if (usableSpans.Any(sp => sp.Min == sp.Max))
            {
                usableSpans = UsableSpans();
            }
            foreach (var span in usableSpans)
            {
                _avSpans.Add(new ElmetroPascalChannelSpan(this, span));
            }

            AvailableSpans = _avSpans.ToArray();
            
            _pressureOperationMode = PressureControllerOperationMode.VENT;

            ActiveSpan = _avSpans.FirstOrDefault(sp => 
                                                        (sp.Scale as ElmetroPascalScale).Module == 1 
                                                      & (sp.Scale as ElmetroPascalScale).RangeNum == 1);
        }

        /// <summary>
        /// Считывает информацию о текущих доступных диапазонах измерений в модулях М1 и М2
        /// возвращает только те, максимум которых не превышает максимум самого маленького модуля
        /// </summary>
        /// <returns></returns>
        internal Collection<ElmetroPascalScale> UsableSpans()
        {
            var Communicator = parentEPascal.Communicator;
            Communicator.QueryCommand("SEEK_MODUL");
            Thread.Sleep(2000);

            var internalModuleRanges = ParseModuleScales(Communicator.QueryCommand("READ_M1?"), 1);
            var externalModuleRanges = ParseModuleScales(Communicator.QueryCommand("READ_M2?"), 2);

            if (!externalModuleRanges.Any()) return internalModuleRanges;

            var internalModuleMaxPressure = internalModuleRanges.OrderByDescending(s => s.Max).FirstOrDefault().Max;
            var externalModuleMaxPressure = externalModuleRanges.OrderByDescending(s => s.Max).FirstOrDefault().Max;

            if (externalModuleMaxPressure > internalModuleMaxPressure)
            {
                externalModuleRanges = new Collection<ElmetroPascalScale>(externalModuleRanges.Where(r => r.Max <= internalModuleMaxPressure).ToList());
            }

            if (internalModuleMaxPressure > externalModuleMaxPressure)
            {
                internalModuleRanges = new Collection<ElmetroPascalScale>(internalModuleRanges.Where(r => r.Max <= externalModuleMaxPressure).ToList());
            }

            return new Collection<ElmetroPascalScale>(internalModuleRanges.Union(externalModuleRanges).OrderBy(s => s.Max).ToArray());

            
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
                        new kPa(),
                        moduleNumber,
                        i + 1
                        )
                    );
                }
            }

            return scales;

        }

        //RANGE n,m
        //n указывает номер модуля (1 - внутренний модуль, 2 - внешний модуль)
        //m – порядковый номер устанавливаемого диапазона(определяется по списку диапазонов из команд READ_M1?, READ_M2?)
        internal bool ActivateSpan(ElmetroPascalChannelSpan epspan)
        {
            if (ActiveSpan != epspan)
            {
                var scale = epspan.Scale as ElmetroPascalScale;
                if (scale != null)
                {
                    var reply = parentEPascal.Communicator.QueryCommand($"RANGE {scale.Module},{scale.RangeNum}");
                    if (reply.Contains("OK"))
                    {
                        Thread.Sleep(2000);
                        ActiveSpan = epspan;
                        _pressureOperationMode = PressureControllerOperationMode.VENT;
                        ActiveSpan.Zero();
                        return true;
                    }
                }
            }
            return false;
        }

        internal void ControlToggle()
        {
            lock (controlPossibleLock)
            {
                string res;
                switch (_pressureOperationMode)
                {
                    case PressureControllerOperationMode.UNKNOWN:
                        res = parentEPascal.Communicator.QueryCommand("ON_KEY_START");
                        Thread.Sleep(500);
                        if (res.Contains("STOP_REGULATION"))
                        {
                            _pressureOperationMode = PressureControllerOperationMode.MEASURE;
                        }
                        if (res.Contains("START_REGULATION"))
                        {
                            _pressureOperationMode = PressureControllerOperationMode.CONTROL;
                        }
                        break;
                    case PressureControllerOperationMode.STANDBY:
                    case PressureControllerOperationMode.MEASURE:
                    case PressureControllerOperationMode.VENT:
                        res = parentEPascal.Communicator.QueryCommand("ON_KEY_START");
                        Thread.Sleep(500);
                        _pressureOperationMode = PressureControllerOperationMode.CONTROL;
                        break;
                    case PressureControllerOperationMode.CONTROL:
                        res = parentEPascal.Communicator.QueryCommand("ON_KEY_START");
                        Thread.Sleep(500);
                        _pressureOperationMode = PressureControllerOperationMode.MEASURE;
                        break;
                    default:
                        break;
                }
            }
        }

        internal void VentToggle()
        {
            lock (controlPossibleLock)
            {
                string res;
                switch (_pressureOperationMode)
                {
                    case PressureControllerOperationMode.UNKNOWN:
                        res = parentEPascal.Communicator.QueryCommand("ON_KEY_VENT");
                        Thread.Sleep(500);
                        if (res == "VENT_ON")
                        {
                            _pressureOperationMode = PressureControllerOperationMode.VENT;
                        }
                        if (res == "VENT_OFF")
                        {
                            _pressureOperationMode = PressureControllerOperationMode.MEASURE;
                        }
                        break;
                    case PressureControllerOperationMode.STANDBY:
                    case PressureControllerOperationMode.MEASURE:
                        parentEPascal.Communicator.QueryCommand("ON_KEY_VENT");
                        Thread.Sleep(500);
                        _pressureOperationMode = PressureControllerOperationMode.VENT;
                        break;
                    case PressureControllerOperationMode.CONTROL:
                        ControlToggle();
                        parentEPascal.Communicator.QueryCommand("ON_KEY_VENT");
                        Thread.Sleep(500);
                        _pressureOperationMode = PressureControllerOperationMode.VENT;
                        break;
                    case PressureControllerOperationMode.VENT:
                        parentEPascal.Communicator.QueryCommand("ON_KEY_VENT");
                        Thread.Sleep(500);
                        _pressureOperationMode = PressureControllerOperationMode.MEASURE;
                        break;
                    default:
                        break;
                }
            }
        }

        internal bool Zero()
        {
            string reply = string.Empty;
            lock (parentEPascal.Communicator)
            {
                reply = parentEPascal.Communicator.QueryCommand(string.Format("CLEAR_P"));
            }
            if (reply.Contains("OK"))
            {
                lock (controlPossibleLock)
                {
                    Thread.Sleep(3000);
                    return true;
                }
            }
            return false;
        }
        public override string ToString()
        {
            var min = AvailableSpans.OrderBy(a => a.Scale.Min).Select(a => a.Scale.Min).FirstOrDefault();
            
            var max = AvailableSpans.OrderByDescending(a => a.Scale.Max).Select(a => a.Scale.Max).FirstOrDefault();

            return String.Format("канал 1: {0} - {1} {2}", min, max, "kPa");
        }


    }
}
