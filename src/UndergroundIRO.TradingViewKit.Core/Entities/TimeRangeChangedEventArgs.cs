using System;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class TimeRangeChangedEventArgs:EventArgs
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}