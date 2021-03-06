﻿using System;
using System.Collections.Generic;
using System.Text;
using JsonSettingsManager.TypeResolving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class NullImplTests
    {
        [ResolveType]
        public interface I
        {

        }

        public class C : I
        {

        }

        public class S
        {
            public I Field { get; set; }
            public I Field2 { get; set; }
        }

        [TestMethod]
        public void Test()
        {
            var manager = new SettingsManager();

            var s = manager.LoadSettings<S>(JToken.Parse("{\"Field\": null, \"Field2\": {\"@Name\": \"C\"}}"));

            Assert.IsNotNull(s);
            Assert.IsNull(s.Field);
            Assert.IsNotNull(s.Field2);

        }
    }
}
