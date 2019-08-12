using JsonSettingsManager.DataSources;

namespace JsonSettingsManager.SpecificProcessors.Options
{
	public class LoadOptions : IStringOption
	{
		public IDataSource DataSource { get; set; }

		public void FillFromString(string str)
		{
			DataSource = new FileDataSource()
			{
				Path = str
			};
		}
	}
}
