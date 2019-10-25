using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using JsonSettingsManager.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
    internal class LoadProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "LoadFrom";
        public bool IsPrefix => false;

        public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
        {
            var options = Common.ParseOptions<LoadOptions>(jOptions, context.Serializer).Single();

            context = context.Clone();

            context.DisableProcessors = options.DisableProcessors ?? false;

            context.Parameters = options.Parameters;

            return context.Manager.LoadSettings(options.DataSource, context, options.Mode);
        }
    }
}
