using System.Linq;
using JsonSettingsManager.DataSources;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.MongoDb
{
	public class MongoDataSource : IDataSource
	{
		public string ConnectionString { get; set; }

		public string DataBase { get; set; }

		public string Collection { get; set; }

		public JToken Filter { get; set; }

		public bool IsArray { get; set; }

		private JObject Make(BsonDocument bson)
		{
			bson.Remove("_id");
			var result = JObject.Parse(bson.ToJson());
			return result;
		}

		public (JToken, IDataSource) Load(IDataSource lastDataSource)
		{
			var client = !string.IsNullOrEmpty(ConnectionString)
				? new MongoClient(ConnectionString)
				: new MongoClient();

			var db = client.GetDatabase(DataBase);

			var collection = db.GetCollection<BsonDocument>(Collection);

			var query = collection.Find(Filter?.ToString() ?? FilterDefinition<BsonDocument>.Empty);

			if (IsArray)
			{
				var items = query.ToList().Select(Make);

				return (new JArray(items), this);
			}

			var item = query.FirstOrDefault();

			if (item == null)
				return (null, this);

			return (Make(item), this);
		}
	}
}
