using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using IRO.XWebView.CefSharp.Utils;

namespace UndergroundIRO.Tests.TradingViewKitWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                Cef.Shutdown();
            };
            //Copied AnyCpu CefSharp example, but it throw exception. 
            //It seems it's because of netcore 3.
            //!Please use x86|x64 configuratian.
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            InitializeCefSharp();
        }

        [STAThread]
        void Application_Startup(object sender, StartupEventArgs e)
        {
            var w = new MainWindow();
            w.Show();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();
            settings.BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86",
                "CefSharp.BrowserSubprocess.exe"
            );
            CefHelpers.AddDefaultSettings(settings);
            settings.RemoteDebuggingPort = 9222;
            Cef.Initialize(settings, false, browserProcessHandler: null);
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
    }
}
