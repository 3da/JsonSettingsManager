using System.Linq;
using JsonSettingsManager.SpecificProcessors;
using JsonSettingsManager.SpecificProcessors.Options;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Templating
{
    public class ConditionProcessor : ISpecificProcessor
    {
        private readonly object _globals;

        public ConditionProcessor(object globals)
        {
            _globals = globals;
        }

        public string KeyWord { get; } = "Condition";
        public bool IsPrefix { get; } = false;
        public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
        {
            var options = Common.ParseOptions<ConditionOptions>(jOptions, context.Serializer).Single();

            var result = CSharpScript.EvaluateAsync<bool>(options.If, globals: _globals).Result;

            return result ? options.Then : options.Else;
        }
    }
}
