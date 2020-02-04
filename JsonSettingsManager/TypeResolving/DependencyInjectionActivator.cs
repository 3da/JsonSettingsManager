using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.TypeResolving
{
    public class DependencyInjectionActivator : JsonConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            object result;
            result = ActivatorUtilities.CreateInstance(_serviceProvider, objectType) ?? Activator.CreateInstance(objectType);


            if (result == null)
            {
                throw new JsonSerializationException("No object created.");
            }

            serializer.Populate(reader, result);
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsPrimitive || objectType == typeof(string))
                return false;
            return true;
        }
    }
}
