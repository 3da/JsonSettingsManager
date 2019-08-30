using System;
using System.Collections.Generic;
using System.Text;
using JsonSettingsManager.Templating;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class TemplatingTests
    {
        [TestMethod]
        public void Test()
        {
            var globals = new Gl { SomeVariable = 1, AnotherVariable = 0 };

            var manager = new SettingsManager(new EvalProcessor(globals), new ConditionProcessor(globals));

            Assert.AreEqual(manager.LoadSettings(@"Data\Templating\ExpectedSettings2.json").ToString(), manager.LoadSettings(@"Data\Templating\Settings.json").ToString());

            globals.SomeVariable = 6;
            globals.AnotherVariable = 0;

            Assert.AreEqual(manager.LoadSettings(@"Data\Templating\ExpectedSettings1.json").ToString(), manager.LoadSettings(@"Data\Templating\Settings.json").ToString());

            globals.AnotherVariable = 1;

            Assert.AreEqual(manager.LoadSettings(@"Data\Templating\ExpectedSettings3.json").ToString(), manager.LoadSettings(@"Data\Templating\Settings.json").ToString());


        }
    }

    public class Gl
    {
        public int SomeVariable { get; set; }
        public int AnotherVariable { get; set; }
    }
}
