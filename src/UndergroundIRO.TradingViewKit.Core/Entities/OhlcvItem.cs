using System;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    [JsonConverter(typeof(OhlcvItemConverter))]
    public class OhlcvItem
    {
        public DateTime DateTime { get; set; }

        public double Open { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Close { get; set; }

        public double? Volume { get; set; }
    }
}