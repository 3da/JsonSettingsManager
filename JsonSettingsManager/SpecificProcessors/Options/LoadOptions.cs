using JsonSettingsManager.DataSources;
using Newtonsoft.Json;

namespace JsonSettingsManager.SpecificProcessors.Options
{
    public class LoadOptions : IStringOption
    {
        public IDataSource DataSource { get; set; }

        public LoadMode Mode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableProcessors { get; set; }

        public void FillFromString(string str)
        {
            DataSource = new FileDataSource()
            {
                Path = str
            };
        }
    }
}
