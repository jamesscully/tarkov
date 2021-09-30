using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace TarkovAssistantWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            {
                Debug.WriteLine(args.Exception);
                Console.Out.WriteLine(args.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            Debug.WriteLine(e.Exception.Message);
            Console.Out.WriteLine(e.Exception.Message);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            Debug.WriteLine((e.ExceptionObject as Exception).Message);
            Console.Out.WriteLine((e.ExceptionObject as Exception).Message);
        }
    }
}
