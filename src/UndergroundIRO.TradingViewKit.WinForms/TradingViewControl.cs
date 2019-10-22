using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IRO.XWebView.CefSharp;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.WinForms;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;
using UndergroundIRO.TradingViewKit.Core;

namespace UndergroundIRO.TradingViewKit.WinForms
{
    public partial class TradingViewControl : UserControl
    {
        public ITradingView TradingView { get; private set; }

        public TradingViewControl()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                try
                {
                    var splashPictureBox = new PictureBox();
                    Controls.Add(splashPictureBox);
                    splashPictureBox.Dock = DockStyle.Fill;
                    splashPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    splashPictureBox.Load("https://raw.githubusercontent.com/ExtremeDotneting/UndergroundIRO.TradingViewKit/master/data/splash.jpg");
                    splashPictureBox.Invalidate();
                }
                catch { }
                return;
            }
            var control = new CefSharpXWebViewControl();
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            var xwv = new CefSharpXWebView(control);
            control.SetVisibilityState(XWebViewVisibility.Visible);
            TradingView = new TradingView(xwv);

        }
    }
}
