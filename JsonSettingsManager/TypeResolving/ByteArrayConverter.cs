using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.TypeResolving
{
    public class ByteArrayConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;

        public override bool CanWrite { get; } = false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = serializer.Deserialize<JToken>(reader);

            if (token.Type == JTokenType.Bytes)
                return new byte[1][] { token.Value<byte[]>() };
            return token.ToObject<byte[][]>();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[][]);
        }
    }
}
