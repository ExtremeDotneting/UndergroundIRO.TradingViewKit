using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using IRO.XWebView.Core.Utils;
using Newtonsoft.Json;
using UndergroundIRO.TradingViewKit.Core;
using UndergroundIRO.TradingViewKit.Core.Entities;

namespace UndergroundIRO.Tests.TradingViewKitWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        long _currentOhlcvTime = 1552471200000;

        readonly long _step;

        ITradingView TradingView { get; }

        public MainWindow()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            //Load data.
            var json = File.ReadAllText("testdata.json");
            var chart = JsonConvert.DeserializeObject<TradingViewChart>(json);
            var ctx = new TradingViewContext()
            {
                Title = "MyTitle",
                Chart = chart
            };
            var lastCandle = chart.Ohlcv.Last();
            _currentOhlcvTime = (long)lastCandle.DateTime.ToUniversalDateTime();
            var prevCandleTime =(long)chart.Ohlcv[chart.Ohlcv.Count - 2].DateTime.ToUniversalDateTime();
            _step = _currentOhlcvTime - prevCandleTime;
            var o = JsonConvert.SerializeObject(lastCandle.Open);
            var h = JsonConvert.SerializeObject(lastCandle.High);
            var l = JsonConvert.SerializeObject(lastCandle.Low);
            var c = JsonConvert.SerializeObject(lastCandle.Close);
            var v = JsonConvert.SerializeObject(lastCandle.Volume);
            TextBox1.Text = $"[{o}, {h}, {l}, {c}, {v}]";
            TradingView = TradingViewCtr.TradingView;
            TradingView.TypedContext = ctx;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _currentOhlcvTime += 3600000;
            try
            {
                var json = TextBox1.Text;
                json = json.Replace("[", $"[{_currentOhlcvTime}, ");
                var newCandle = JsonConvert.DeserializeObject<OhlcvItem>(json);
                if (newCandle == null)
                    throw new System.Exception();
                TradingView.TypedContext.Chart.Ohlcv.Add(newCandle);
            }
            catch
            {
                _currentOhlcvTime -= 3600000;
                throw;
            }
        }
    }
}
