using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench
{
    public class StabilityCalculator
    {
        public readonly List<OneMeasure> Measures;

        readonly List<OneMeasure> StableMeasures;
        public int MeasuresCount { get => Measures.Count; }
        public int MinMeasuresCount { get; set; }
        public bool Ready { get => StableMeasures.Count >= MinMeasuresCount; }
        public double StableMeanValue { get => StableMeasures.Average(m => m.Value); }
        public TimeSpan MeasuringTimeSpan { get; private set; }
        public TimeSpan MinTimeToStabilize { get; set; }

        public TrendStatus TrendStatus { get; private set; }
        public double MeanValue { get => Measures.Average(m => m.Value); }
        public double StdDeviation { get; private set; }
        public double LRSlope { get; private set; }
        public StabilityCalculator(int minMeasuresCount, TimeSpan minTimeToStabilize)
        {
            MinMeasuresCount = minMeasuresCount;
            MinTimeToStabilize = minTimeToStabilize;
            TrendStatus = TrendStatus.Unknown;
            Measures  = new List<OneMeasure>();
            StableMeasures  = new List<OneMeasure>();
            MeasuringTimeSpan = TimeSpan.FromSeconds(0);
        }
        public void Reset()
        {
            TrendStatus = TrendStatus.Unknown;
            Measures.Clear();
            StableMeasures.Clear();
            MeasuringTimeSpan = TimeSpan.FromSeconds(0);
        }
        public void AddMeasure(OneMeasure oneMeasure)
        {
            Measures.Add(oneMeasure);
            DateTime mintimestamp;
            TimeSpan timespan;

            mintimestamp = Measures.Min(cm => cm.TimeStamp); 
            timespan = DateTime.Now - mintimestamp;

            while (timespan.TotalSeconds > MinTimeToStabilize.TotalSeconds & Measures.Count > MinMeasuresCount)
            {
                Measures.RemoveAt(0);
                mintimestamp = Measures.Min(cm => cm.TimeStamp);
                timespan = DateTime.Now - mintimestamp;

            }

            MeasuringTimeSpan = timespan;

            var averageValue = Measures.Average((om) => om.Value);

            StdDeviation = Math.Sqrt(Measures.Sum(meas => Math.Pow( meas.Value - averageValue, 2))) ;

            LRSlope = LRSlopeCalc(Measures);

            if (MeasuresCount >= MinMeasuresCount)
            {

                if (Math.Round(LRSlope, 3) == 0)
                {
                    TrendStatus = TrendStatus.Stable;
                    StableMeasures.Add(oneMeasure);
                }
                else
                {
                    StableMeasures.Clear();
                    if (LRSlope < 0)
                    {
                        TrendStatus = TrendStatus.FallDown;
                    }
                    else if (LRSlope > 0)
                    {
                        TrendStatus = TrendStatus.GrowUP;
                    }
                }
            }
            else
            {
                StableMeasures.Clear();
                TrendStatus = TrendStatus.Unknown;
            }

        }
        private static double LRSlopeCalc(List<OneMeasure> oneMeasureResults)
        {
            double x = 1;
            double xtotal = 0;
            double y = 0;
            double ytotal = 0;
            double XmultY = 0;
            double XmultYtotal = 0;
            double Xsqr = 0;
            double Xsqrtotal = 0;
            foreach (var item in oneMeasureResults)
            {
                xtotal += x;
                ytotal += item.Value;
                XmultY = item.Value * x;
                XmultYtotal += XmultY;
                Xsqr = Math.Pow(x, 2);
                Xsqrtotal += Xsqr;
                x++;
            }
            x--;
            double b = (x * XmultYtotal - xtotal * ytotal) / (x * Xsqrtotal - Math.Pow(xtotal, 2));
            return b;
        }
    }
}
