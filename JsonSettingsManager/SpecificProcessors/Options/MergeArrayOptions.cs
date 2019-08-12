using JsonSettingsManager.DataSources;

namespace JsonSettingsManager.SpecificProcessors.Options
{
	public class MergeArrayOptions : IStringOption
	{
		public IDataSource DataSource { get; set; }
		public void FillFromString(string str)
		{
			DataSource = new FileDataSource() { Path = str };
		}
	}
}
