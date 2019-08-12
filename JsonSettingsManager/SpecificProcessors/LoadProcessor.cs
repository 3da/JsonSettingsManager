using System;
using System.Collections.Generic;
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

			return context.Manager.LoadSettings(options.DataSource, context);
		}
	}
}
