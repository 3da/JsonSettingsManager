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
    [Ignore]
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
    }

}
