using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Templating
{
    public class ConditionOptions
    {
        public JToken Then { get; set; }

        public JToken Else { get; set; }

        public string If { get; set; }
    }
}
