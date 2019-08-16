using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace JsonSettingsManager.Serialization
{
    public class SerializationContext
    {
        public delegate void SaveExternalHandler(SerializationContext context, string path, object value);

        public event SaveExternalHandler SaveExternal;

        public string WorkDir { get; set; }

        public void OnSaveExternal(SerializationContext context, string path, object value)
        {
            SaveExternal?.Invoke(context, Path.Combine(WorkDir, path), value);
        }
    }
}
