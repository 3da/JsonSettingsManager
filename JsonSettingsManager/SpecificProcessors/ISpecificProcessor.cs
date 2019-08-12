using JsonSettingsManager.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
	internal interface ISpecificProcessor
	{
		string KeyWord { get; }

		bool IsPrefix { get; }

		JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord);
	}
}