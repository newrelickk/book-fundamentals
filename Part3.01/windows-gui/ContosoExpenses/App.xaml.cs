using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ContosoExpenses
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(@"C:\log.txt"));
            Trace.AutoFlush = true;

            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, ex) =>
            {
                Trace.WriteLine(ex.Exception);
            };
        }
    }
}
