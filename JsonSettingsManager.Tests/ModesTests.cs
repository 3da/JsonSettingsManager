using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class ModesTests
    {
        [TestMethod]
        public async Task Test()
        {
            var manager = new SettingsManager();
            var settings = await manager.LoadSettingsAsync(@"Data\LoadModes\Settings.json");

            Assert.AreEqual(JToken.Parse(await File.ReadAllTextAsync(@"Data\LoadModes\Expected.json")).ToString(), settings.ToString());
        }
    }
}
