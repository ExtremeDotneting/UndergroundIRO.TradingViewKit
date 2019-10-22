using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IRO.EmbeddedResources;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;
using Newtonsoft.Json;
using UndergroundIRO.TradingViewKit.Core.Entities;

namespace UndergroundIRO.TradingViewKit.Core
{
    /// <summary>
    /// Interaction logic for TradingViewControl.xaml
    /// </summary>
    public class TradingView : ITradingView
    {
        #region Static.
        static string UnpackedToPath { get; set; }
        public static bool IsUnpacked { get; set; }

        /// <summary>
        /// Ignored if unpacked.
        /// Must be called before first TradingView created.
        /// </summary>/param>
        /// <returns></returns>
        public static void UnpackResources(string unpackedToPath = null)
        {
            if (IsUnpacked)
                return;
            unpackedToPath ??= Path.Combine(Environment.CurrentDirectory, "WebRes");
            if (!Directory.Exists(unpackedToPath))
                Directory.CreateDirectory(unpackedToPath);
            var embeddedDirPath = "UndergroundIRO.TradingViewKit.Core.TradingVueApp";
            var assembly = Assembly.GetExecutingAssembly();
            assembly.ExtractEmbeddedResourcesDirectory(embeddedDirPath, unpackedToPath);
            UnpackedToPath = unpackedToPath;
            IsUnpacked = true;
        }
        #endregion

        string _previousRefreshJson;

        public IXWebView XWV { get; }
        public virtual TradingViewContext TypedContext { get; set; }
        public bool IsDisposed { get; set; }
        public bool LoopRefreshEnabled { get; set; } = true;
        /// <summary>
        /// Default is 50 ms.
        /// </summary>
        public TimeSpan LoopRefreshTimeout { get; set; } = TimeSpan.FromMilliseconds(50);

        public event Action<ITradingView, TimeRangeChangedEventArgs> TimeRangeChanged;

        public TradingView(IXWebView xwv)
        {
            UnpackResources();
            XWV = xwv ?? throw new ArgumentNullException(nameof(xwv));
            XWV.Visibility = XWebViewVisibility.Visible;

            var thread = new Thread(async (obj) =>
            {
                while (!IsDisposed)
                {
                    try
                    {
                        await Task.Delay(LoopRefreshTimeout);
                        if (LoopRefreshEnabled)
                        {
                            var ctx = XWV.ThreadSync.Invoke(() => TypedContext);
                            await RefreshAsync();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
            Action<double, double> timeRangeChangedHandler = (double startTime, double endTime) =>
            {
                var startDateTime = TimeExtensions.FromUniversalDateTime(startTime);
                var endDateTime = TimeExtensions.FromUniversalDateTime(endTime);
                var args=new TimeRangeChangedEventArgs()
                {
                    StartTime = startDateTime,
                    EndTime = endDateTime
                };
                XWV.ThreadSync.Invoker.Invoke(() =>
                {
                    TimeRangeChanged?.Invoke(this, args); 
                });
            };
            xwv.BindToJs(timeRangeChangedHandler, "timeRangeUpdated", "window");
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            XWV.Dispose();
            IsDisposed = true;
        }

        public async Task RefreshAsync(ViewRefreshType viewRefreshType = ViewRefreshType.NotReloadIfCached)
        {
            await OnRefreshing(this, viewRefreshType);
            var ctx = XWV.ThreadSync.Invoke(() => TypedContext);
            string ctxJson = null;
            var chartJson = JsonConvert.SerializeObject(ctx.Chart);
            var titleJson = JsonConvert.SerializeObject(ctx.Title);
            var colors = JsonConvert.SerializeObject(ctx.Colors);
            if (viewRefreshType == ViewRefreshType.NotReloadIfCached)
            {
                ctxJson = JsonConvert.SerializeObject(ctx);
                if (ctxJson == _previousRefreshJson)
                    return;
            }

            var script = $@"
TradingVueObj.titleTxt = {titleJson} ;
TradingViewContext.chart = {chartJson} ;
TradingViewContext.colors = {colors} ;
";
            await XWV.WaitInitialization();
            if (viewRefreshType == ViewRefreshType.ReloadAllPage)
            {
                await LoadPage();
            }
            else
            {
                await LoadPageIfNotLoaded();
            }
            await XWV.ExJs<object>(script);
            _previousRefreshJson = ctxJson;
            await OnRefreshed(this, viewRefreshType);
        }

        /// <summary>
        /// From first to last candle.
        /// </summary>
        /// <returns></returns>
        public async Task SetDefaultTimeRange()
        {
            var ctx = XWV.ThreadSync.Invoke(() => TypedContext);
            var from = ctx.Chart.Ohlcv.First().DateTime;
            var to = ctx.Chart.Ohlcv.Last().DateTime;
            await SetTimeRange(from, to);

        }

        public async Task SetTimeRange(DateTime from, DateTime to)
        {
            var fromNum = (long)from.ToUniversalDateTime();
            var toNum = (long)to.ToUniversalDateTime();
            await SetTimeRange(fromNum, toNum);

        }

        public async Task SetTimeRange(long fromNum, long toNum)
        {

            var script = $@"
TradingVueObj.setRange({fromNum},{toNum});
";
            await XWV.ExJs<object>(script);
        }

        public async Task<(DateTime StartTime, DateTime EndTime)> GetTimeRange()
        {

            var script = $@"
return TradingVueObj.getRange();
";
            var arr=await XWV.ExJs<double[]>(script);
            var startDT = TimeExtensions.FromUniversalDateTime(arr[0]);
            var endDT = TimeExtensions.FromUniversalDateTime(arr[1]);
            return (startDT, endDT);
        }

        async Task LoadPageIfNotLoaded()
        {
            var isLoadedScript = @"
if(window['TradingViewContext'])
  return true;
return false;
";
            var loaded = await XWV.ExJs<bool>(isLoadedScript);
            if (loaded)
                return;
            await LoadPage();
        }

        async Task LoadPage()
        {
            var path = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                UnpackedToPath + "/dist/index.html"
            );
            await XWV.LoadUrl(path);
            await XWV.AttachBridge();
        }

        #region Awaitable events.
        readonly List<Func<ITradingView, ViewRefreshType, Task>> _refreshingEvents = new List<Func<ITradingView, ViewRefreshType, Task>>();
        public event Func<ITradingView, ViewRefreshType, Task> Refreshing
        {
            add { _refreshingEvents.Add(value); }
            remove { _refreshingEvents.Remove(value); }
        }

        async Task OnRefreshing(ITradingView sender, ViewRefreshType viewRefreshType)
        {
            foreach (var item in _refreshingEvents)
            {
                var t = item(sender, viewRefreshType);
                if (t != null)
                {
                    await t;
                }
            }
        }

        readonly List<Func<ITradingView, ViewRefreshType, Task>> _refreshedEvents = new List<Func<ITradingView, ViewRefreshType, Task>>();
        public event Func<ITradingView, ViewRefreshType, Task> Refreshed
        {
            add { _refreshedEvents.Add(value); }
            remove { _refreshedEvents.Remove(value); }
        }

        async Task OnRefreshed(ITradingView sender, ViewRefreshType viewRefreshType)
        {
            foreach (var item in _refreshedEvents)
            {
                var t = item(sender, viewRefreshType);
                if (t != null)
                {
                    await t;
                }
            }
        }
        #endregion
    }
}