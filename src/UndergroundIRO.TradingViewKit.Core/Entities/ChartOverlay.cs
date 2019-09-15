using System.Collections.Generic;
using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class ChartOverlay
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// EMA, RSI ...
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

         [JsonProperty("data")]
        public ICollection<TradingOverlayItem> Data { get; set; }

         [JsonProperty("settings")]
        public IDictionary<string, object> Settings { get; set; }
    }
}