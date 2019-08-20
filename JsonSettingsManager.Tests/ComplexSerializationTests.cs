using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using JsonSettingsManager.DataSources;
using JsonSettingsManager.Serialization;
using JsonSettingsManager.Serialization.Attributes;
using JsonSettingsManager.TypeResolving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class ComplexSerializationTests
    {
        [ResolveType]
        interface IUnit
        {
            IList<IUnit> Items { get; set; }
        }

        class Dog : IUnit
        {
            public IList<IUnit> Items { get; set; }
        }

        class Human : IUnit
        {
            public IList<IUnit> Items { get; set; }
        }


        [TestMethod]
        public void TestTypeNameSerialization()
        {
            var data = new Human()
            {
                Items = new List<IUnit>()
                {
                    new Dog(),
                    new Human()
                    {
                        Items = new List<IUnit>()
                        {
                            new Dog(), new Dog()
                        }
                    }
                }
            };

            var serializer = new SettingsSerializer();

            serializer.SaveJson(data, "TmpResult.json");

            SerializeTests.CompareJsons(@"Data\SerializationDeep\ExpectedSettings.json", "TmpResult.json");

            var manager = new SettingsManager();

            var loadedData = manager.LoadSettings<Human>("TmpResult.json");

            Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(loadedData));
        }


        class Settings
        {
            public string Field { get; set; }

            [ToExternal(Path = "Ext{id}.json")]
            public Settings ExternalJson { get; set; }

            [ToExternal(Path = "Data{id}.bin", Mode = LoadMode.Bin)]
            public byte[] ExternalBin { get; set; }
        }

        readonly Settings _settings = new Settings()
        {
            Field = "Value",
            ExternalJson = new Settings()
            {
                Field = "Value2",
                ExternalBin = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x70 }
            },
            ExternalBin = new byte[] { 0, 1, 2, 3, 4, 5 }
        };

        [TestMethod]
        public void TestMultipleIntances()
        {

            var tmpDir = new DirectoryInfo("Tmp");
            if (tmpDir.Exists)
            {
                tmpDir.Delete(true);
                Thread.Sleep(100);
            }

            var serializer = new SettingsSerializer();

            serializer.SaveJson(_settings, @"Tmp\settings.json");

            SerializeTests.CompareJsons(@"Data\SerializationDeep2\ExpectedSettings.json", @"Tmp\settings.json");
            SerializeTests.CompareJsons(@"Data\SerializationDeep2\Ext0000.json", @"Tmp\Ext0000.json");
            SerializeTests.CompareBytes(@"Data\SerializationDeep2\Data0001.bin", @"Tmp\Data0001.bin");
            SerializeTests.CompareBytes(@"Data\SerializationDeep2\Data0002.bin", @"Tmp\Data0002.bin");
        }

        [TestMethod]
        public void TestMultipleIntancesZip()
        {

            var tmpDir = new DirectoryInfo("Tmp");
            if (tmpDir.Exists)
            {
                tmpDir.Delete(true);
                Thread.Sleep(100);
            }

            var serializer = new SettingsSerializer();

            serializer.SaveZip(_settings, @"Tmp.zip");

            ZipFile.ExtractToDirectory("Tmp.zip", "Tmp");

            SerializeTests.CompareJsons(@"Data\SerializationDeep2\Main.json", @"Tmp\Main.json");
            SerializeTests.CompareJsons(@"Data\SerializationDeep2\Ext0000Zip.json", @"Tmp\Ext0000.json");
            SerializeTests.CompareBytes(@"Data\SerializationDeep2\Data0001.bin", @"Tmp\Data0001.bin");
            SerializeTests.CompareBytes(@"Data\SerializationDeep2\Data0002.bin", @"Tmp\Data0002.bin");
        }
    }
}
