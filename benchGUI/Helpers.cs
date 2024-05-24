using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WorkBench;
using WorkBench.Enums;

namespace benchGUI
{
    internal class Helpers
    {
    }
    static class TrendStatusConverter
    {
        public static string GetStatusTextRu(this StabilityCalculator stabilityCalculator)
        {
            return stabilityCalculator.TrendStatus switch
            {
                TrendStatus.Unknown => "неизвестно",
                TrendStatus.GrowUP => "увеличивается",
                TrendStatus.FallDown => "уменьшается",
                TrendStatus.Stable => "стабильно",
                _ => throw new ArgumentException($"unknown trendstatus {stabilityCalculator.TrendStatus}")
            };
        }

        public static string ToWBFloatString(this double value)
        {
            return $"{value:0.0000}";
        }

        public static double ParseToDouble(this string value)
        {
            value.TryParseToDouble(out double result);
            return result;
        }

        public static bool TryParseToDouble(this string value, out double result)
        {
            return double.TryParse(
                    value.Replace(',', '.'),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out result);
        }

        public static bool IsFloatString(this string value)
        {
            return value.TryParseToDouble(out _);
        }
    }
}
