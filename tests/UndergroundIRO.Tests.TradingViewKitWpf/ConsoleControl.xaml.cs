using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using IRO.CmdLine;
using IRO.CmdLine.OnXWebView;
using IRO.Storage.DefaultStorages;
using IRO.XWebView.CefSharp;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.Wpf;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;

namespace UndergroundIRO.Tests.TradingViewKitWpf
{
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl
    {
        readonly CefSharpXWebViewControl _control;

        public CefSharpXWebView XWV { get; }

        public CmdLineExtension Cmd { get; private set; }

        public bool IsInit { get; private set; }

        public ConsoleControl()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            _control = new CefSharpXWebViewControl();
            _control.SetVisibilityState(XWebViewVisibility.Visible);
            Content = _control;
            XWV = new CefSharpXWebView(_control);
            Unloaded += delegate { Dispose(); };
        }

        public async Task Initialize(string terminalStorageName = "storage")
        {
            await _control.CurrentBrowser.WaitInitialization();
            var ch = new TerminalJsConsoleHandler(XWV);
            var storage = new FileStorage(terminalStorageName);
            Cmd = new CmdLineExtension(ch, storage);
            XWV.LoadFinished += delegate
            {
                try
                {
                    //XWV.SetZoomLevel(0.8);
                }
                catch
                {
                }
            };
            await XWV.TerminalJs().LoadTerminalIfNotLoaded();
            IsInit = true;
        }

        public async Task RunAsync(Func<CmdLineExtension, CommandLineBase> commandLineBaseFactory)
        {
            if (commandLineBaseFactory == null)
                throw new ArgumentNullException(nameof(commandLineBaseFactory));
            await Task.Run(async () =>
            {
                if (!IsInit)
                {
                    await Initialize();
                }

                var cmdSwitcher = new CmdSwitcher();
                var commandLineBase = commandLineBaseFactory(Cmd);
                cmdSwitcher.PushCmdInStack(commandLineBase);
                //Note that this method block current thread and can't be used from ui thread.
                //Enter 'q' to exit.
                cmdSwitcher.RunDefault();
                Dispose();
            });
        }

        public void Dispose()
        {
            ThreadSync.Inst.TryInvoke(() =>
            {
                XWV.Dispose();
                this.Content = null;
            });
        }
    }
}
