using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UndergroundIRO.TradingViewKit.Core.Entities
{
    public class OhlcvItemConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (OhlcvItem)value;
            var arr = new object[]
            {
                (long)obj.DateTime.ToUniversalDateTime(),
                obj.Open,
                obj.High,
                obj.Low,
                obj.Close,
                obj.Volume
            };
            serializer.Serialize(writer, arr);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (objectType != typeof(OhlcvItem))
            {
                throw new NotImplementedException("Custom json converter wrong type.");
            }
            var jToken = JToken.Load(reader);
            var arr = jToken.ToObject<double[]>();
            var res = new OhlcvItem();
            if (arr.Length > 0)
                res.DateTime = TimeExtensions.FromUniversalDateTime(arr[0]);
            if (arr.Length > 1)
                res.Open = arr[1];
            if (arr.Length > 2)
                res.High = arr[2];
            if (arr.Length > 3)
                res.Low = arr[3];
            if (arr.Length > 4)
                res.Close = arr[4];
            if (arr.Length > 5)
                res.Volume = arr[5];
            return res;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OhlcvItem);
        }
    }
}