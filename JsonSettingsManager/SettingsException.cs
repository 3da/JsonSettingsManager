using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager
{
    public class SettingsException : Exception
    {
        public JToken JToken { get; set; }

        public SettingsException()
        {
        }

        public SettingsException(string message) : base(message)
        {
        }

        public SettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override string ToString()
        {
            return JToken.ToString(Formatting.None) + base.ToString();
        }
    }
}
