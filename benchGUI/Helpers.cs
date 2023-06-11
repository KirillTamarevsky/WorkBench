using System;
using System.Collections.Generic;
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
    }
}
