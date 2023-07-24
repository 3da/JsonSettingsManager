using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonSettingsManager.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
    internal class LoadProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "LoadFrom";
        public bool IsPrefix => false;

        public async Task<JToken> DoAsync(ParseContext context, JToken jOptions, JObject obj, string keyWord,
            CancellationToken token = default)
        {
            var options = Common.ParseOptions<LoadOptions>(jOptions, context.Serializer).Single();

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

            return await context.Manager.LoadSettingsAsync(options.DataSource, context, options.Mode, token);
        }
    }
}
