﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace benchGUI
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            log4net.Config.XmlConfigurator.Configure();
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
