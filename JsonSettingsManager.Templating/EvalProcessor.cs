using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonSettingsManager.SpecificProcessors;
using JsonSettingsManager.SpecificProcessors.Options;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Templating
{
    public class EvalProcessor : ISpecificProcessor
    {
        private readonly object _globals;

        public EvalProcessor(object globals)
        {
            _globals = globals;
        }

        public string KeyWord { get; } = "Eval";
        public bool IsPrefix { get; } = false;
        public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
        {
            var options = Common.ParseOptions<EvalOptions>(jOptions, context.Serializer).Single();

            try
            {

                var result = CSharpScript.EvaluateAsync(options.Expression, globals: _globals).Result;


                return result != null ? JToken.FromObject(result) : null;
            }
            catch (Exception e)
            {
                throw new SettingsException($"Error processing: {options.Expression}")
                {
                    JToken = jOptions
                };
            }


        }
    }
}
