using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
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
        public bool UseSplashScreen { get; set; } = true;

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
            BrowserPlace.Content = control;
            var xwv = new CefSharpXWebView(control);
            TradingView = new TradingView(xwv);
            Unloaded += delegate { TradingView.Dispose(); };
            LoadingImageGrid.Visibility = UseSplashScreen ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            TradingView.Refreshing += async (tv, refreshType) =>
            {
                if (refreshType == ViewRefreshType.ReloadAllPage && UseSplashScreen)
                {
                    await tv.XWV.ThreadSync.InvokeAsync(() =>
                    {
                        LoadingImageGrid.Visibility = System.Windows.Visibility.Visible;
                    });
                }
            };

            TradingView.Refreshed += async (tv, refreshType) =>
            {
                await tv.XWV.ThreadSync.InvokeAsync(() =>
                {
                    LoadingImageGrid.Visibility = System.Windows.Visibility.Collapsed;
                });
            };
        }

        #region Xaml props.
        public static readonly DependencyProperty OverWidthProperty =
        DependencyProperty.RegisterAttached(nameof(UseSplashScreen), typeof(bool), typeof(TradingViewControl), new PropertyMetadata(true));

        public static void SetUseSplashScreen(UIElement element, bool value)
        {
            element.SetValue(OverWidthProperty, value);
        }

        public static bool GetUseSplashScreen(UIElement element)
        {
            return (bool)element.GetValue(OverWidthProperty);
        }
        #endregion

    }
}
