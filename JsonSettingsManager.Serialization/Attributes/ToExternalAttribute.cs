using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSettingsManager.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ToExternalAttribute : Attribute
    {
        public string Path { get; set; }
    }
}
