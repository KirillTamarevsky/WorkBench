using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench
{
    public class StabilityCalculator
    {
        public readonly List<OneMeasure> Measures;

        readonly List<OneMeasure> StableMeasures;
        public int MeasuresCount
        {
            get
            {
                lock (Measures)
                {
                    return Measures.Count;
                }
            }
        }
        public int MinMeasuresCount { get; set; }
        public bool Ready
        {
            get
            {
                lock (StableMeasures)
                {
                    return StableMeasures.Count >= MinMeasuresCount;
                }
            }
        }
        public double StableMeanValue
        {
            get
            {
                lock (StableMeasures)
                {
                    return StableMeasures.Average(m => m.Value);
                }
            }
        }
        public TimeSpan MeasuringTimeSpan { get; private set; }
        public TimeSpan MinTimeToStabilize { get; set; }

        public TrendStatus TrendStatus { get; private set; }
        public double MeanValue { get; private set; }
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
            lock (Measures)
            {
                lock (StableMeasures)
                {
                    TrendStatus = TrendStatus.Unknown;
                    Measures.Clear();
                    StableMeasures.Clear();
                    MeasuringTimeSpan = TimeSpan.FromSeconds(0);
                }
            }
        }
        public void AddMeasure(OneMeasure oneMeasure)
        {
            lock (Measures)
            {
                lock (StableMeasures)
                {
                    if (oneMeasure == null) return;
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

                    MeanValue = Measures.Average((om) => om.Value);

                    StdDeviation = Math.Sqrt(Measures.Sum(meas => Math.Pow(meas.Value - MeanValue, 2)));

                    LRSlopeCalc();

                    if (MeasuresCount >= MinMeasuresCount)
                    {

                        if (Math.Round(LRSlope, 3, MidpointRounding.ToZero) == 0)
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
            }
        }
        private void LRSlopeCalc()
        {
            double x = 1;
            double xtotal = 0;
            //double y = 0;
            double ytotal = 0;
            double XmultYtotal = 0;
            double Xsqrtotal = 0;
            foreach (var item in Measures)
            {
                xtotal += x;
                ytotal += item.Value;
                double XmultY = item.Value * x;
                XmultYtotal += XmultY;
                double Xsqr = Math.Pow(x, 2);
                Xsqrtotal += Xsqr;
                x++;
            }
            x--;
            LRSlope = (x * XmultYtotal - xtotal * ytotal) / (x * Xsqrtotal - Math.Pow(xtotal, 2));
        }
    }
}
