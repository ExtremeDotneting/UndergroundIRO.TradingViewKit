using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class TradeOverlayItemConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (TradeOverlayItem)value;
            var list = new List<object>()
            {
                (long)obj.DateTime.ToUniversalDateTime(),
                obj.Type,
                obj.Price
            };
            if (obj.Label != null)
            {
                list.Add(obj.Label);
            }
            serializer.Serialize(writer, list);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (objectType != typeof(TradeOverlayItem))
            {
                throw new NotImplementedException("Custom json converter wrong type.");
            }
            JToken jToken = JToken.Load(reader);
            var arr = jToken.ToObject<JToken[]>();
            var res = new TradeOverlayItem();
            res.DateTime = TimeExtensions.FromUniversalDateTime(arr[0].ToObject<double>());
            res.Price = arr[1].ToObject<double>();
            res.Type = arr[2].ToObject<TradeMarkerType>();
            if(arr.Length>3)
                res.Label = arr[3].ToObject<string>();
            return res;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TradeOverlayItem);
        }
    }
}