using System;
using System.Collections.Generic;
using System.Text;
using JsonSettingsManager.DataSources;
using Newtonsoft.Json;

namespace JsonSettingsManager
{
    public class ParseContext : ICloneable
    {
        public IDataSource DataSource { get; set; }
        public bool MergeArray { get; set; }
        public SettingsManager Manager { get; set; }
        public JsonSerializer Serializer { get; set; }
        public bool DisableProcessors { get; set; }

        public ParseContext Clone()
        {
            return new ParseContext()
            {
                DataSource = DataSource,
                Manager = Manager,
                MergeArray = MergeArray,
                Serializer = Serializer,
                DisableProcessors = DisableProcessors
            };
        }

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
}
