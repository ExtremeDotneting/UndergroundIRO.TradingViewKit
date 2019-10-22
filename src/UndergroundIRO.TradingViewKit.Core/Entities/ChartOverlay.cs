using System.Collections.Generic;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class ChartOverlay
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// EMA, RSI, Volume, Candles, Splines, Channel, Range, Trades, Segment, Splitters.
        /// <para></para>
        /// See https://github.com/C451/trading-vue-js/tree/master/docs/overlays#built-in-overlays .
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Use *OverlayItem or JToken.
        /// </summary>
        [JsonProperty("data")]
        public IList<object> Data { get; set; }

        [JsonProperty("settings")]
        public IDictionary<string, object> Settings { get; set; }
    }
}