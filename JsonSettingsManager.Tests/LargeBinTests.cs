using System.Linq;
using JsonSettingsManager.DataSources;
using JsonSettingsManager.Serialization;
using JsonSettingsManager.Serialization.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonSettingsManager.Tests
{
    [TestClass]
    public class LargeBinTests
    {
        public class Settings
        {
            [ToExternal(Mode = LoadMode.LargeBin, Path = "Content_{id}.bin")]
            public byte[][] Content { get; set; }
            public string Message { get; set; }
        }

        [TestMethod]
        public void MakeLargeZip()
        {
            var megabyteBuf = Enumerable.Range(0, 1024 * 1024).Select(q => (byte)(q % 256)).ToArray();

            var megabyteCount = 1024 * 5; //5GB

            var totalSize = megabyteCount * 1024L * 1024L;

            var settings = new Settings()
            {
                Message = "Total size: " + totalSize,
                Content = Enumerable.Range(0, megabyteCount).Select(_ => megabyteBuf).ToArray()
            };

            var serializer = new SettingsSerializer();
            serializer.SaveZip(settings, "Large.zip");


            var settingsManager = new JsonSettingsManager.SettingsManager();

            var result = settingsManager.LoadSettingsZip<Settings>("Large.zip");

            Assert.AreEqual(settings.Message, result.Message);
            Assert.AreEqual(totalSize, result.Content.Sum(q => q.LongLength));
            Assert.AreEqual(0, result.Content[0][0]);
            Assert.AreEqual(255, result.Content.Last().Last());
        }
    }
}
