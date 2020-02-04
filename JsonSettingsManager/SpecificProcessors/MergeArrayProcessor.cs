using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonSettingsManager.DataSources;
using JsonSettingsManager.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
    internal class MergeArrayProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "MergeArrayWith";
        public bool IsPrefix => false;

        public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
        {
            var options = Common.ParseOptions<MergeArrayOptions>(jOptions, context.Serializer).Single();

            context.MergeArray = true;

            context = context.Clone();

            context.DisableProcessors = options.DisableProcessors ?? false;

            if (context.Parameters == null)
            {
                context.Parameters = options.Parameters;
            }
            else if (options.Parameters != null)
            {
                foreach (KeyValuePair<string, object> optionParameter in options.Parameters)
                {
                    context.Parameters[optionParameter.Key] = optionParameter.Value;
                }
            }

            JToken otherToken;

            otherToken = context.Manager.LoadSettings(options.DataSource, context, LoadMode.Json);

            var otherArray = otherToken as JArray;

            if (otherArray == null)
                throw new SettingsException($"Merge path {options.DataSource} must be JArray");



            return otherArray;
        }
    }
}
