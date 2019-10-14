using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public bool LoadSplashScreenVisible
        {
            get
            {
                return LoadingImageGrid.Visibility == Visibility.Visible;
            }
            set
            {
                LoadingImageGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public TradingViewControl()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                try
                {
                    var img = new Image();
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(
                        "https://raw.githubusercontent.com/ExtremeDotneting/UndergroundIRO.TradingViewKit/master/data/splash.jpg",
                        UriKind.Absolute
                        );
                    bitmap.EndInit();
                    img.Stretch = Stretch.Fill;
                    img.Source = bitmap;
                    Content = img;
                }
                catch { }
                return;
            }
            var control = new CefSharpXWebViewControl();
            BrowserPlace.Content = control;
            var xwv = new CefSharpXWebView(control);
            TradingView = new TradingView(xwv);
            Unloaded += delegate { TradingView.Dispose(); };
            LoadingImageGrid.Visibility = UseSplashScreen ? Visibility.Visible : Visibility.Collapsed;

            TradingView.Refreshing += async (tv, refreshType) =>
            {
                if (refreshType == ViewRefreshType.ReloadAllPage && UseSplashScreen)
                {
                    await tv.XWV.ThreadSync.InvokeAsync(() =>
                    {
                        LoadSplashScreenVisible = true;
                    });
                }
            };

            TradingView.Refreshed += async (tv, refreshType) =>
            {
                await tv.XWV.ThreadSync.InvokeAsync(() =>
                {
                    LoadSplashScreenVisible = false;
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
