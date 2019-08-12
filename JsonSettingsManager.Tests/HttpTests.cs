using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Tests
{
	[TestClass]
	public class HttpTests
	{

		[TestMethod]
		public void Test()
		{
			var settingsManager = new SettingsManager();

			var settings = settingsManager.LoadSettings(@"Data\HttpTest\Settings.json");

			var expectedSettings = JToken.Parse(File.ReadAllText(@"Data\HttpTest\ExpectedSettings.json"));

			Assert.IsTrue(JToken.DeepEquals(expectedSettings, settings));
		}
	}
}
