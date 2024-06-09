using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkBench;

namespace benchGUI
{
    public partial class MainForm //: Form
    {
        Task PlotRefresherTask { get; set; }
        CancellationTokenSource PlotRefresherCTS { get; set; } = new CancellationTokenSource();
        Task OnStartDemoTask { get; set; }
        CancellationTokenSource OnStartDemoCTS { get; set; } = new CancellationTokenSource();

        Axis XTimeAxis { get; set; }
        Axis YmAAxis { get; set; }
        Axis YPressureAxis { get; set; }

        ScatterPlot currentMeasuresScatterPlot { get; set; }
        ScatterPlot pressureMeasuresScatterPlot;
        private ScatterPlotList<double> resultScatter { get; set; }

        List<OneMeasure> currentMeasures = new List<OneMeasure>();
        List<OneMeasure> pressureMeasures = new List<OneMeasure>();

        private void InitPlot() 
        {
            plot_result.Plot.Clear();
            XTimeAxis = plot_result.Plot.AddAxis(Edge.Bottom);
            YmAAxis = plot_result.Plot.AddAxis(Edge.Right);
            YPressureAxis = plot_result.Plot.AddAxis(Edge.Right);

            XTimeAxis.DateTimeFormat(true);
            XTimeAxis.IsVisible = false;

            // inti Standard Measuring Instruments Readings Plot
            currentMeasuresScatterPlot = new ScatterPlot(new double[] { 0 }, new double[] { 0 });
            currentMeasuresScatterPlot.Color = Color.DarkGray;
            currentMeasuresScatterPlot.LineStyle = LineStyle.Dot;
            plot_result.Plot.Add(currentMeasuresScatterPlot);
            currentMeasuresScatterPlot.XAxisIndex = XTimeAxis.AxisIndex;
            currentMeasuresScatterPlot.YAxisIndex = YmAAxis.AxisIndex;
            pressureMeasuresScatterPlot = new ScatterPlot(new double[] { 0 }, new double[] { 0 });
            pressureMeasuresScatterPlot.Color = Color.DarkGray;
            pressureMeasuresScatterPlot.LineStyle = LineStyle.DashDotDot;
            plot_result.Plot.Add(pressureMeasuresScatterPlot);
            pressureMeasuresScatterPlot.XAxisIndex = XTimeAxis.AxisIndex;
            pressureMeasuresScatterPlot.YAxisIndex = YPressureAxis.AxisIndex;

            plot_result.Plot.XAxis.TickLabelFormat((d) => $"{d}%");
            plot_result.Plot.SetAxisLimitsY(0, 1);

            OnStartDemoCTS = new CancellationTokenSource();
            OnStartDemoTask = Task.Run(() => OnStartDemoLoop(OnStartDemoCTS.Token));

            PlotRefresherCTS = new CancellationTokenSource();
            PlotRefresherTask = Task.Run(() => PlotRefresherLoop(PlotRefresherCTS.Token));

        }

        private void OnStartDemoLoop(CancellationToken token)
        {
            Random rand = new Random(1234);
            double[] xs = DataGen.Consecutive(25, 4, offset: 1d); // new double[] { 0, 25, 50, 75, 100};
            InvokeControlAction(() =>
            {
                //plot_result.Plot.Clear();
                for (int i = 0; i < 2; i++)
                {
                    double[] ys = DataGen.RandomNormal(rand, xs.Length, 0.5, 0.00);
                    var scatter = plot_result.Plot.AddScatter(xs, ys);
                    scatter.Smooth = true;
                    scatter.MarkerShape = (MarkerShape)Enum.GetValues(typeof(MarkerShape)).GetValue(new Random().Next(0, Enum.GetValues(typeof(MarkerShape)).Length - 1));
                }
            });
            do
            {
                InvokeControlAction(() =>
                {
                    foreach (var plottable in plot_result.Plot.GetPlottables())
                    {
                        if (plottable != currentMeasuresScatterPlot & plottable != pressureMeasuresScatterPlot && plottable is ScatterPlot scatt)
                        {
                            var ys = scatt.Ys;
                            for (int i = 0; i < ys.Length; i++)
                            {
                                ys[i] += (rand.NextDouble() - 0.5) / 600;
                                if (ys[i] > 1) ys[i] = 0.9;
                                if (ys[i] < 0) ys[i] = 0.1;
                            }
                        }
                    }
                });
                PlotRefreshEvent.WaitOne(500);
                Task.Delay(1).Wait();

            } while (!token.IsCancellationRequested);
            var flattened = false;
            do
            {
                InvokeControlAction(() =>
                {
                    foreach (var plottable in plot_result.Plot.GetPlottables())
                    {
                        plot_result.Plot.MoveLast(plottable);
                        if (plottable != currentMeasuresScatterPlot & plottable != pressureMeasuresScatterPlot && plottable is ScatterPlot scatt)
                        {
                            var ys = scatt.Ys;
                            for (int i = 0; i < ys.Length; i++)
                            {
                                if (ys[i] > 0.5) ys[i] -= (ys[i] - 0.5) / rand.Next(3, 15);
                                if (ys[i] < 0.5) ys[i] += (0.5 - ys[i]) / rand.Next(3, 15);
                            }
                            if (ys.All(y => y >= 0.49 & y <= 0.51)) flattened = true;
                            else flattened = false;
                        }
                        //plot_result.Refresh(skipIfCurrentlyRendering: true);
                    }
                });
                PlotRefreshEvent.WaitOne(500);
            } while (!flattened);
            InvokeControlAction(() =>
            {
                foreach (var p in plot_result.Plot.GetPlottables().Where(p => p != currentMeasuresScatterPlot & p != pressureMeasuresScatterPlot))
                { plot_result.Plot.Remove(p); }
            });
        }
        private EventWaitHandle PlotRefreshEvent = new ManualResetEvent(false );
        private void PlotRefresherLoop(CancellationToken token)
        {
            Stopwatch stopwatch = new Stopwatch();
            do
            {
                InvokeControlAction(() =>
                    {
                        double xAxisMinLimit = DateTime.Now.AddSeconds(-TIMETOSTABLE).ToOADate();
                        double xAxisMaxLimit = DateTime.Now.ToOADate();
                        plot_result.Plot.SetAxisLimitsX(xAxisMinLimit, xAxisMaxLimit, xAxisIndex: XTimeAxis.AxisIndex);

                        stopwatch.Restart();
                        plot_result.Refresh(lowQuality: true);
                        stopwatch.Stop();
                        PlotRefreshEvent.Set();
                    });
                PlotRefreshEvent.WaitOne(500);
                PlotRefreshEvent.Reset();
                var elapsedms = (int)(stopwatch.ElapsedMilliseconds);
                Task.Delay(elapsedms * 2).Wait();
                //Thread.Sleep(elapsedms  * 2);
            } while (!token.IsCancellationRequested);
        }

    }
}
