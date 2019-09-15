using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using IRO.CmdLine;
using IRO.XWebView.Core.Utils;
using Newtonsoft.Json;
using UndergroundIRO.TradingViewKit.Core;
using UndergroundIRO.TradingViewKit.Core.Entities;
using UndergroundIRO.TradingViewKit.Wpf;

namespace UndergroundIRO.Tests.TradingViewKitWpf
{
    public class TestsCmdLine : CommandLineBase
    {
        public TestsCmdLine(CmdLineExtension cmdLineExtension) : base(cmdLineExtension)
        {
        }

        [CmdInfo]
        public void TradingViewEntitiesSerializationTest()
        {
            var json = File.ReadAllText("testdata.json");
            var obj = JsonConvert.DeserializeObject<TradingViewChart>(json);
            var outputJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
            //json = json.Replace(" ", "").Replace("\n", "");
            //outputJson = outputJson.Replace(" ", "").Replace("\n", "");
            File.WriteAllText("inputJson.json", json);
            File.WriteAllText("outputJson.json", outputJson);
            Process.Start("inputJson.json");
            Process.Start("outputJson.json");
        }

        #region TestTradingView.
        long _currentOhlcvTime = 1552471200000;

        [CmdInfo]
        public void TestTradingView()
        {
            //Load data.
            var json = File.ReadAllText("testdata.json");
            var chart = JsonConvert.DeserializeObject<TradingViewChart>(json);
            var ctx = new TradingViewContext()
            {
                Title="MyTitle",
                Chart = chart
            };
            var lastCandle = chart.Ohlcv.Last();
            _currentOhlcvTime =(long)lastCandle.DateTime.ToUniversalDateTime();

            //Init ui.
            Window window = null;
            TradingViewControl tradingViewControl = null;
            bool windowOpened = true;
            ThreadSync.Inst.Invoke(() =>
            {
                window = new Window();
                tradingViewControl = new TradingViewControl();
                tradingViewControl.TradingView.TypedContext = ctx;
                window.Content = tradingViewControl;
                window.Closed += delegate
                {
                    windowOpened = false;
                };
                window.Show();

            });

            var o = JsonConvert.SerializeObject(lastCandle.Open);
            var h = JsonConvert.SerializeObject(lastCandle.High);
            var l = JsonConvert.SerializeObject(lastCandle.Low);
            var c = JsonConvert.SerializeObject(lastCandle.Close);
            var v = JsonConvert.SerializeObject(lastCandle.Volume);
            Cmd.WriteLine(
                $"Enter ohlcv, example: '[{o}, {h}, {l}, {c}, {v}]'."
                );
            while (windowOpened)
            {
                try
                {
                    var str = Cmd.ReadLine();
                    var newCandle = CrunchOhlcvParser(str);
                    if (newCandle == null)
                        throw new System.Exception();
                    chart.Ohlcv.Add(newCandle);
                    Cmd.WriteLine("Refreshed.");
                }
                catch
                {
                    Cmd.WriteLine("Can't parse this.");
                }
            }

            ThreadSync.Inst.TryInvoke(() => { window.Close(); });
        }

        OhlcvItem CrunchOhlcvParser(string json)
        {
            _currentOhlcvTime += 3600000;
            try
            {
                json = json.Replace("[", $"[{_currentOhlcvTime}, ");
                return JsonConvert.DeserializeObject<OhlcvItem>(json);
            }
            catch
            {
                _currentOhlcvTime -= 3600000;
                throw;
            }
        }
        #endregion
    }
}
