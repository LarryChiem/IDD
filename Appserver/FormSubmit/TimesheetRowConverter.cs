using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
public class TimesheetRowConverter: JsonConverter
{
    public override bool CanConvert(Type objectType){ return false;}
    public override bool CanWrite{ get {return false;}}
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Object)
        {
            JToken results = token["results"];
            if (results != null && results.Type == JTokenType.Array){
                return results.ToObject<List<TimesheetRowItem>>(serializer);
            }
            else if( results == null)
            {
                return new List<TimesheetRowItem> {token.ToObject<TimesheetRowItem>(serializer)};
            }
        }
        throw new JsonSerializationException("Timesheet Row Converter JSON error");
    }
}