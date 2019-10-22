using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using UndergroundIRO.TradingViewKit.Core;
using UndergroundIRO.TradingViewKit.Core.Entities;

namespace UndergroundIRO.Tests.TradingViewKitWinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            //Load data.
            var json = File.ReadAllText("testdata.json");
            var chart = JsonConvert.DeserializeObject<TradingViewChart>(json);
            var lastCandle = chart.Ohlcv.Last();

            //Add trade marker.
            var tradeOverlayItem = new TradeOverlayItem()
            {
                DateTime = lastCandle.DateTime,
                Price = lastCandle.Close,
                Type = TradeMarkerType.Sell,
                Label = "My trade"
            };
            var tradesChartOverlay = new ChartOverlay()
            {
                Name="Maked trades",
                Type = "Trades",
                Data = new List<object>
                {
                    tradeOverlayItem
                },
                Settings =new Dictionary<string, object>()
            };
            tradesChartOverlay.Settings["z-index"] = 1;
            chart.OnChart.Add(tradesChartOverlay);

            var ctx = new TradingViewContext()
            {
                Title = "MyTitle",
                Chart = chart
            };
            var tv = tradingViewControl1.TradingView;
            tv.TypedContext = ctx;
            tv.TimeRangeChanged += async (s, e) =>
            {
                Debug.WriteLine($"StartTime: {e.StartTime}, EndTime: {e.EndTime}");

            };
            Task.Run(async () =>
            {
                await Task.Delay(3000);
                Debug.WriteLine(JsonConvert.SerializeObject(await tv.GetTimeRange()));
            });
        }
    }
}
