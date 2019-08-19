using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class ModesTests
    {
        [TestMethod]
        public void Test()
        {
            var manager = new SettingsManager();
            var settings = manager.LoadSettings(@"Data\LoadModes\Settings.json");

            Assert.AreEqual(JToken.Parse(File.ReadAllText(@"Data\LoadModes\Expected.json")).ToString(), settings.ToString());
        }
    }
}
