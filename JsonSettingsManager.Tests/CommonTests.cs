using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Tests
{
	[TestClass]
	public class CommonTests
	{
		private void Test(string expectedPath, string actualPath)
		{
			var settingsManager = new SettingsManager();

			var settings = settingsManager.LoadSettings(actualPath);

			var expectedSettings = JToken.Parse(File.ReadAllText(expectedPath));

			Assert.IsTrue(JToken.DeepEquals(expectedSettings, settings));
		}

		[TestMethod]
		public void TestComplexSettings()
		{
			Test("Data\\ComplexTest\\ExpectedSettings.json", "Data\\ComplexTest\\Settings.json");

		}

		[TestMethod]
		public void TestMergeTwoTrees()
		{
			Test("Data\\TwoTrees\\ExpectedSettings.json", "Data\\TwoTrees\\Settings.json");
		}
	}
}
