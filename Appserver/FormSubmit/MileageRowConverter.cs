using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Appserver.FormSubmit
{
    public class MileageRowConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType) { return false; }
        public override bool CanWrite { get { return false; } }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                JToken results = token["results"];
                if (results != null && results.Type == JTokenType.Array)
                {
                    return results.ToObject<List<MileageRowItem>>(serializer);
                }
                else if (results == null)
                {
                    return new List<MileageRowItem> { token.ToObject<MileageRowItem>(serializer) };
                }
            }
            throw new JsonSerializationException("Mileage Row Converter JSON error");
        }
    }
}
