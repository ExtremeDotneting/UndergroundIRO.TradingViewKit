using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using Newtonsoft.Json.Linq;
using UndergroundIRO.TradingViewKit.Core.Entities;

namespace UndergroundIRO.TradingViewKit.Core
{
    public interface ITradingView
    {
        IXWebView XWV { get; }

        TradingViewContext TypedContext { get; set; }

        bool IsDisposed { get; set; }

        bool LoopRefreshEnabled { get; set; }

        /// <summary>
        /// Default is 300 ms.
        /// </summary>
        TimeSpan LoopRefreshTimeout { get; set; }

        /// <summary>
        /// All tasks will be awaited.
        /// </summary>
        event Func<ITradingView, ViewRefreshType, Task> Refreshing;

        /// <summary>
        /// All tasks will be awaited.
        /// </summary>
        event Func<ITradingView, ViewRefreshType, Task> Refreshed;

        void Dispose();

        Task RefreshAsync(ViewRefreshType viewRefreshType = ViewRefreshType.NotReloadIfCached);

        /// <summary>
        /// From first to last candle.
        /// </summary>
        /// <returns></returns>
        Task SetDefaultTimeRange();

        Task SetTimeRange(DateTime from, DateTime to);

        Task SetTimeRange(long fromNum, long toNum);

       
    }
}