using System.ComponentModel;
using System.Globalization;
using System.Threading;
using IRO.XWebView.CefSharp;
using IRO.XWebView.CefSharp.Wpf;
using UndergroundIRO.TradingViewKit.Core;

namespace UndergroundIRO.TradingViewKit.Wpf
{
    /// <summary>
    /// Interaction logic for TradingViewControl.xaml
    /// </summary>
    public partial class TradingViewControl 
    {
        public ITradingView TradingView { get; }

        public TradingViewControl()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            var control = new CefSharpXWebViewControl();
            Content = control;
            var xwv = new CefSharpXWebView(control);
            TradingView = new TradingView(xwv);
            Unloaded += delegate { TradingView.Dispose(); };
        }

        
    }
}
