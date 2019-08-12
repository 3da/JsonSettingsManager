using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors.Options
{
	public interface IStringOption
	{
		void FillFromString(string str);
	}
}