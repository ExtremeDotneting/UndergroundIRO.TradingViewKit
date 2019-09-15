using System;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    [JsonConverter(typeof(TradingOverlayItemConverter))]
    public class TradingOverlayItem
    {
        public DateTime DateTime { get; set; }

        public double Value { get; set; }
    }
}