using System;
using System.Collections.Generic;
using System.Text;
using JsonSettingsManager.SpecificProcessors.Options;

namespace JsonSettingsManager.Templating
{
    public class EvalOptions : IStringOption
    {
        public string Expression { get; set; }
        public void FillFromString(string str)
        {
            Expression = str;
        }
    }
}
