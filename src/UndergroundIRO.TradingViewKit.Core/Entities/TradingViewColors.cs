using Newtonsoft.Json;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class TradingViewColors
    {
        [JsonProperty("colorBack")]
        public string ColorBack { get; set; } = "#fff";

        [JsonProperty("colorGrid")]
        public string ColorGrid { get; set; } = "#eee";

        [JsonProperty("colorText")]
        public string ColorText { get; set; } = "#333";
    }
}