using System;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    [JsonConverter(typeof(OverlayItemConverter))]
    public class OverlayItem
    {
        public DateTime DateTime { get; set; }

        public double Value { get; set; }
    }
}