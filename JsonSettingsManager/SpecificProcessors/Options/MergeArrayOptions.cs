using JsonSettingsManager.DataSources;
using Newtonsoft.Json;

namespace JsonSettingsManager.SpecificProcessors.Options
{
    public class MergeArrayOptions : IStringOption
    {
        public IDataSource DataSource { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableProcessors { get; set; }

        public void FillFromString(string str)
        {
            DataSource = new FileDataSource() { Path = str };
        }
    }
}
