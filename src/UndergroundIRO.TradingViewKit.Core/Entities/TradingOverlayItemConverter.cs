using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class  TradingOverlayItemConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (TradingOverlayItem)value;
            var arr = new object[]
            {
               (long)obj.DateTime.ToUniversalDateTime(),
                obj.Value
            };
            serializer.Serialize(writer, arr);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (objectType != typeof(TradingOverlayItem))
            {
                throw new NotImplementedException("Custom json converter wrong type.");
            }
            JToken jToken = JToken.Load(reader);
            var arr = jToken.ToObject<double[]>();
            var res = new TradingOverlayItem();
            if (arr.Length > 0)
                res.DateTime = TimeExtensions.FromUniversalDateTime(arr[0]);
            if (arr.Length > 1)
                res.Value = arr[1];
            return res;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TradingOverlayItem);
        }
    }
}