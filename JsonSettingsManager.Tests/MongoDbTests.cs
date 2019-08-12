using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using JsonSettingsManager.MongoDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Tests
{
	[TestClass]
	public class MongoDbTests
	{
		private const string MongoDbName = "JsonSettings_" + nameof(MongoDbTests);



		[TestMethod]
		public void Test()
		{
			ModuleSettings.Initialize();

			var client = new MongoClient();

			var db = client.GetDatabase(MongoDbName);

			db.DropCollection(nameof(Test));

			var collection = db.GetCollection<BsonDocument>(nameof(Test));
			
			collection.InsertMany(new[]
			{
				BsonDocument.Parse(@"
{
	""Hello"": ""World"",
	""Test"": 123,
	""F"":1
}
"),
				BsonDocument.Parse(@"
{
	""A"": ""B"",
	""Test"": [""Z"", 1, null],
	""F"":2
}
"),
			});


			var settingsManager = new SettingsManager();

			var settings = settingsManager.LoadSettings(@"Data\MongoDbTest\Settings.json");

			var expectedSettings = JToken.Parse(File.ReadAllText(@"Data\MongoDbTest\ExpectedSettings.json"));

			Assert.IsTrue(JToken.DeepEquals(expectedSettings, settings));
		}

	}
}
