using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using JsonSettingsManager.Serialization;
using JsonSettingsManager.Serialization.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class SerializeTests
    {
        class Settings
        {
            public string Title { get; set; }

            [ToExternal(Path = "Users")]
            public string[] Users { get; set; }

            [ToExternal(Path = "Roles")]
            public string[] Roles { get; set; }
        }

        private void CompareJsons(string path1, string path2)
        {
            Assert.IsTrue(JToken.DeepEquals(JToken.Parse(File.ReadAllText(path1)), JToken.Parse(File.ReadAllText(path2))));

        }

        [TestMethod]
        public void TestSerializeJson()
        {
            var settings = new Settings()
            {
                Title = "Hello",
                Roles = new[] { "Admin", "User", "Guest" },
                Users = new[] { "John", "Jane", "Jet" }
            };

            var serializer = new SettingsSerializer();

            var tmpDir = new DirectoryInfo("Tmp");
            if (tmpDir.Exists)
            {
                tmpDir.Delete(true);
                Thread.Sleep(100);
            }

            serializer.SaveJson(settings, @"Tmp\settings.json");

            Assert.IsTrue(File.Exists(@"Tmp\settings.json"));
            Assert.IsTrue(File.Exists(@"Tmp\Users.json"));
            Assert.IsTrue(File.Exists(@"Tmp\Roles.json"));

            CompareJsons(@"Data\Serialization\ExpectedSettings.json", @"Tmp\settings.json");
            CompareJsons(@"Data\Serialization\Users.json", @"Tmp\Users.json");
            CompareJsons(@"Data\Serialization\Roles.json", @"Tmp\Roles.json");

            var manager = new SettingsManager();
            var loadedSettings = manager.LoadSettings(@"Tmp\settings").ToObject<Settings>();

            Assert.AreEqual(JsonConvert.SerializeObject(settings), JsonConvert.SerializeObject(loadedSettings));
        }
    }
}
