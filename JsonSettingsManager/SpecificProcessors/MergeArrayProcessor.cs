using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var otherArray = context.Manager.LoadSettings(options.DataSource, context) as JArray;

			if (otherArray == null)
				throw new Exception($"Merge path {options.DataSource} must be JArray");

			context.MergeArray = true;

			return otherArray;
		}
	}
}
