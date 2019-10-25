using System;
using System.Collections.Generic;
using System.Text;
using JsonSettingsManager.DataSources;
using JsonSettingsManager.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
    internal class MergeProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "MergeWith";
        public bool IsPrefix => false;

        public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
        {
            var options = Common.ParseOptions<MergeOptions>(jOptions, context.Serializer);

            JObject resultObj = obj;

            foreach (var option in options)
            {
                var jsonMergeSettings = new JsonMergeSettings
                {
                    MergeNullValueHandling = option.NullValueHandling,
                    MergeArrayHandling = MergeArrayHandling.Replace,
                    PropertyNameComparison =
                        option.IgnoreCase
                            ? StringComparison.OrdinalIgnoreCase
                            : StringComparison.Ordinal
                };

                var mergePath = option.DataSource;

                context = context.Clone();

                context.DisableProcessors = option.DisableProcessors ?? false;

                context.Parameters = option.Parameters;

                var otherToken = context.Manager.LoadSettings(mergePath, context, LoadMode.Json);

                var otherObj = otherToken as JObject;

                if (otherObj == null)
                    throw new SettingsException($"Merge path {mergePath} must be JObject");


                if (option.Priority == MergePriority.This)
                {
                    otherObj.Merge(resultObj, jsonMergeSettings);
                    resultObj = otherObj;
                }
                else if (option.Priority == MergePriority.Other)
                {
                    resultObj.Merge(otherObj, jsonMergeSettings);
                }
                else
                    throw new NotImplementedException(option.Priority.ToString());
            }

            return resultObj;
        }
    }
}
