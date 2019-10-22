using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.WinForms.Utils;
using IRO.XWebView.Core.Utils;

namespace UndergroundIRO.Tests.TradingViewKitWinForms
{
    static class Program
    {
        static Form HiddenForm { get; set; }

        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            //AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InitializeCefSharp();
            //In example we use invisible main form as synchronization context.
            //It's important for ThreadSync that main form must be available during all app lifetime.
            HiddenForm = new Form();
            HiddenForm.FormBorderStyle = FormBorderStyle.None;
            HiddenForm.ShowInTaskbar = false;
            HiddenForm.Load += delegate
            {
                HiddenForm.Size = new Size(0, 0);
                ApplicationStartup();
            };
            XWebViewThreadSync.Init(new WinFormsThreadSyncInvoker(HiddenForm));
            Application.Run(HiddenForm);

        }

        [STAThread]
        static void ApplicationStartup()
        {
            var form = new MainForm();
            form.FormClosed += delegate { HiddenForm.Close(); };
            form.Show();

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void InitializeCefSharp()
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
    }
}
