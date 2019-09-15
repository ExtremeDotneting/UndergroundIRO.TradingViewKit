using System.Collections.Generic;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class TradingViewChart
    {
        [JsonProperty("ohlcv")]
        public IList<OhlcvItem> Ohlcv { get; set; }

        [JsonProperty("onchart")]
        public IList<ChartOverlay> OnChart { get; set; }

        [JsonProperty("offchart")]
        public IList<ChartOverlay> OffChart { get; set; }
    }
}
