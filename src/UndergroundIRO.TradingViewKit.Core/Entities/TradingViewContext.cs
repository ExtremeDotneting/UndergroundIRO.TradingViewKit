﻿namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class TradingViewContext
    {
        public string Title { get; set; } = "";

        public TradingViewChart Chart { get; set; }

        public TradingViewColors Colors { get; set; } = new TradingViewColors();
    }
}