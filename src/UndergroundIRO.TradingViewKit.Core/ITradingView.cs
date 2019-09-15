using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using UndergroundIRO.TradingViewKit.Core.Entities;

namespace UndergroundIRO.TradingViewKit.Core
{
    public interface ITradingView
    {
        TradingViewContext TypedContext { get; set; }

        bool IsDisposed { get; set; }

        bool LoopRefreshEnabled { get; set; }

        /// <summary>
        /// Default is 300 ms.
        /// </summary>
        TimeSpan LoopRefreshTimeout { get; set; }

        event EventHandler Refreshed;

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