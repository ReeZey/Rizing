using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
namespace Rizing.Save {
    public class smolQuaternion : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WritePropertyName("z"); writer.WriteValue(value.z);
            writer.WritePropertyName("w"); writer.WriteValue(value.w);
            writer.WriteEndObject();
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Quaternion((float)obj["x"], (float)obj["y"], (float)obj["z"], (float)obj["w"]);
        }
    }
}