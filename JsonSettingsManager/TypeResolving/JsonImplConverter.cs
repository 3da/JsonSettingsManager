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
                {
                    if (prop.Converter != null)
                    {
                        var writer1 = resultObj.CreateWriter();
                        writer1.WritePropertyName(prop.PropertyName);
                        prop.Converter.WriteJson(writer1, propValue, serializer);
                    }
                    else
                    {
                        resultObj[prop.PropertyName] = JToken.FromObject(propValue, serializer);
                    }

                }
            }

            serializer.Serialize(writer, resultObj);
        }

        private static IList<Type> GetTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies.SelectMany(q =>
            {
                try
                {
                    return q.GetTypes();
                }
                catch
                {
                    return Enumerable.Empty<Type>();
                }
            }).ToArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var types = (objectType.IsInterface
                ? GetTypes()
                    .Where(p => objectType.IsAssignableFrom(p))
                : GetTypes()
                    .Where(t => t.IsSubclassOf(objectType) || objectType == t)).ToArray();

            Type type;

            JObject jObject = JObject.Load(reader);

            if (types.Length == 1)
            {
                type = types[0];
            }
            else if (types.Length == 0)
            {
                throw new Exception($"No classes implement {objectType.Name}");
            }
            else
            {
                var classNameProp = jObject.Property("@Name");

                classNameProp.Remove();

                var className = classNameProp.Value.Value<string>();

                type = types.FirstOrDefault(q => q.Name.Equals(className));

                if (type == null)
                    throw new Exception($"Cannot find implementation '{className}' of '{objectType.Name}'");
            }

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
