﻿using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace benchGUI
{
    public partial class MainForm : Form
    {
        ScatterPlot currentMeasuresScatterPlot;
        ScatterPlot pressureMeasuresScatterPlot;

        private Task GetOnStartDemoTask()
        {
            return Task.Run(() =>
            {
                Random rand = new Random(1234);
                double[] xs = DataGen.Consecutive(25, 4, offset: 1d); // new double[] { 0, 25, 50, 75, 100};
                Thread.Sleep(500);
                InvokeControlAction(() =>
                {
                    //plot_result.Plot.Clear();
                    for (int i = 0; i < 25; i++)
                    {
                        double[] ys = DataGen.RandomNormal(rand, xs.Length, 0.5, 0.00);
                        var scatter = plot_result.Plot.AddScatter(xs, ys);
                        scatter.Smooth = true;
                        scatter.MarkerShape = (MarkerShape)Enum.GetValues(typeof(MarkerShape)).GetValue(new Random().Next(0, Enum.GetValues(typeof(MarkerShape)).Length - 1));
                    }
                });
                var msBetweenFrames = 1000 / 30;
                do
                {
                    InvokeControlAction(() =>
                    {
                        //var plottable = plot_result.Plot.GetPlottables().First();
                        //plot_result.Plot.MoveLast(plottable);
                        foreach (var plottable in plot_result.Plot.GetPlottables())
                        {

                            if (plottable != currentMeasuresScatterPlot & plottable != pressureMeasuresScatterPlot && plottable is ScatterPlot scatt)
                            {
                                var ys = scatt.Ys;
                                for (int i = 0; i < ys.Length; i++)
                                {
                                    ys[i] += (rand.NextDouble() - 0.5) / 100;
                                    if (ys[i] > 1) ys[i] = 0.9;
                                    if (ys[i] < 0) ys[i] = 0.1;
                                }
                            }
                        }
                        //plot_result.Refresh(skipIfCurrentlyRendering: true);
                    });
                    Thread.Sleep(msBetweenFrames);
                } while (!OnStartDemoCTS.IsCancellationRequested);
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
                    Thread.Sleep(1000 / 60);
                } while (!flattened);
                InvokeControlAction(() => {
                    foreach (var p in plot_result.Plot.GetPlottables().Where(p => p != currentMeasuresScatterPlot & p != pressureMeasuresScatterPlot))
                    { plot_result.Plot.Remove(p); }
                });
            });
        }
        private Task GetPlotRefresherTask()
        {
            return Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                do
                {
                    InvokeControlAction(() =>
                    {
                        stopwatch.Restart();
                        plot_result.Refresh(lowQuality: true);
                        stopwatch.Stop();
                        manualResetEvent.Set();
                    });
                    manualResetEvent.WaitOne(500);
                    Thread.Sleep((int)(stopwatch.ElapsedMilliseconds) * 2);
                    manualResetEvent.Reset();
                } while (!PlotRefresherCTS.Token.IsCancellationRequested);
            });
        }

    }
}
