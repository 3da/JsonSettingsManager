using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonSettingsManager.TypeResolving
{
    public class JsonImplConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();

            var typeName = type.Name;

            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(type);

            var resultObj = new JObject
            {
                ["@Name"] = typeName
            };

            foreach (var prop in contract.Properties)
            {
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue != null)
                    resultObj[prop.PropertyName] = JToken.FromObject(propValue, serializer);
            }

            serializer.Serialize(writer, resultObj);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var types = objectType.IsInterface
                ? AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => objectType.IsAssignableFrom(p))
                : AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => t.IsSubclassOf(objectType));

            JObject jObject = JObject.Load(reader);

            var classNameProp = jObject.Property("@Name");

            classNameProp.Remove();

            var className = classNameProp.Value.Value<string>();

            var type = types.FirstOrDefault(q => q.Name.Equals(className));

            if (type == null)
                throw new Exception($"Cannot find implementation '{className}' of '{objectType.Name}'");

            var result = Activator.CreateInstance(type);

            serializer.Populate(jObject.CreateReader(), result);

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetCustomAttribute(typeof(ResolveTypeAttribute), true) != null
                || objectType.GetInterfaces().Any(i => i.GetCustomAttribute(typeof(ResolveTypeAttribute)) != null);
        }
    }
}
