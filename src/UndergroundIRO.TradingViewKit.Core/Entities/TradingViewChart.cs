using System.Collections.Generic;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class TradingViewChart
    {
        [JsonProperty("ohlcv")]
        public ICollection<OhlcvItem> Ohlcv { get; set; }

        [JsonProperty("onchart")]
        public ICollection<ChartOverlay> OnChart { get; set; }

        [JsonProperty("offchart")]
        public ICollection<ChartOverlay> OffChart { get; set; }
    }
}
