using System;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    [JsonConverter(typeof(TradeOverlayItemConverter))]
    public class TradeOverlayItem
    {
        public DateTime DateTime { get; set; }

        public TradeMarkerType Type { get; set; }

        public double Price { get; set; }

        public string Label { get; set; }
    }

}