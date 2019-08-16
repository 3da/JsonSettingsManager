using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using JsonSettingsManager.Serialization.Attributes;
using Newtonsoft.Json;

namespace JsonSettingsManager.Serialization
{
    public class SettingsSerializer
    {
        public void SaveZip(object obj, string path)
        {

        }



        public void SaveJson(object obj, string path)
        {
            var type = obj.GetType();

            var props = type.GetProperties()
                .Where(q => q.GetMethod.IsPublic && q.GetCustomAttribute(typeof(JsonIgnoreAttribute)) == null)
                .ToArray();


            var context = new SerializationContext();
            context.SaveExternal += SaveExternal;

            SaveExternal(context, path, obj);

            context.SaveExternal -= SaveExternal;
        }

        private void SaveExternal(SerializationContext context, string path, object value)
        {
            var fi = new FileInfo(path);
            if (fi.Extension.Length == 0)
                path = path + ".json";



            fi = new FileInfo(path);

            var dir = fi.Directory;

            if (!dir.Exists)
                dir.Create();

            var newContext = new SerializationContext()
            {
                WorkDir = dir.FullName
            };

            newContext.SaveExternal += SaveExternal;

            var text = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver = new MySolver(newContext)
            });

            File.WriteAllText(path, text);

            newContext.SaveExternal -= SaveExternal;
        }
    }
}
