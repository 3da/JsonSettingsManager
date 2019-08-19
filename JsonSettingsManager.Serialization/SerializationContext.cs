using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using JsonSettingsManager.DataSources;
using Newtonsoft.Json;

namespace JsonSettingsManager.Serialization
{
    public class SerializationContext
    {
        public SerializationContext(IWriter writer)
        {
            Writer = writer;
        }

        public delegate void SaveExternalHandler(SerializationContext context, string path, LoadMode mode, object value);

        public event SaveExternalHandler SaveExternal;

        public string WorkDir { get; set; }

        public GlobalContext GlobalContext { get; set; }

        public IWriter Writer { get; }

        public void OnSaveExternal(SerializationContext context, string path, LoadMode mode, object value)
        {
            SaveExternal?.Invoke(context, Path.Combine(WorkDir, path), mode, value);
        }
    }
}
