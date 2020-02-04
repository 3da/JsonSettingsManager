using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using JsonSettingsManager.Templating;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class TemplatingTests2
    {
        [TestMethod]
        public void Test2()
        {
            var globals = new Gl { };

            var manager = new SettingsManager(new EvalProcessor(globals), new ConditionProcessor(globals));

            Assert.IsTrue(File.Exists("Data\\Templating2\\Data.bin"));

            var ss1 = manager.LoadSettings(@"Data\Templating2\Settings.json").ToString();

            Assert.IsTrue(File.Exists("Data\\Templating2\\Data.bin"));

        }

        [TestMethod]
        public void TestLoadWithParameters()
        {
            var manager = new SettingsManager(new EvalProcessor(null), new ConditionProcessor(null));

            var settings = manager.LoadSettings("Data\\LoadWithParams\\Settings.json");

            var settings2 = manager.LoadSettings("Data\\LoadWithParams\\Expected.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }

        [TestMethod]
        public void TestLoadWithParameters2()
        {
            var globals = new Dictionary<string, object>()
            {
                ["Var1"] = 10,
                ["Var2"] = true
            };

            var manager = new SettingsManager(new EvalProcessor(globals), new ConditionProcessor(globals));

            var settings = manager.LoadSettings("Data\\LoadWithParams\\External.json");

            var settings2 = manager.LoadSettings("Data\\LoadWithParams\\Expected2.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }

        [TestMethod]
        public void TestLoadWithParameters3()
        {
            var globals = new Dictionary<string, object>()
            {
                ["Var1"] = 0.33333333333333333f,
                ["Var2"] = true
            };

            var manager = new SettingsManager(new EvalProcessor(globals), new ConditionProcessor(globals));

            var settings = manager.LoadSettings("Data\\LoadWithParams\\Settings3.json");

            var settings2 = manager.LoadSettings("Data\\LoadWithParams\\Expected3.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }

        [TestMethod]
        public void TestLoadWithParameters4()
        {
            var globals = new Dictionary<string, object>()
            {
            };

            var manager = new SettingsManager(new EvalProcessor(globals));

            var settings = manager.LoadSettings("Data\\LoadWithParams\\A4.json");

            var settings2 = manager.LoadSettings("Data\\LoadWithParams\\E4.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }
    }

}
