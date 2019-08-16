using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Serialization
{
    public class ExternalJsonConverter : JsonConverter
    {
        private readonly string _externalAttrPath;
        private readonly SerializationContext _context;

        public ExternalJsonConverter(string externalAttrPath, SerializationContext context)
        {
            _externalAttrPath = externalAttrPath;
            _context = context;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            _context.OnSaveExternal(_context, _externalAttrPath, value);
            serializer.Serialize(writer, new JObject()
            {
                ["@LoadFrom"]= _externalAttrPath
            });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new InvalidOperationException();
        }
    }
}
