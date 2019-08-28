using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSettingsManager.DocumentationLib.Tests
{
    public class Settings
    {
        public SubSettings SubSettings { get; set; }

        public decimal Number { get; set; }

        public string Str { get; set; }

        public MyEnum[] MyEnums { get; set; }
    }
}
