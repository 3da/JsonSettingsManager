using System;
using JsonSettingsManager.DataSources;

namespace JsonSettingsManager.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ToExternalAttribute : Attribute
    {
        public string Path { get; set; }

        public LoadMode Mode { get; set; }
    }
}
