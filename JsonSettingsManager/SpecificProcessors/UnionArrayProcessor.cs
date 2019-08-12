using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
	internal class UnionArrayProcessor : ISpecificProcessor
	{
		public string KeyWord => "union-";
		public bool IsPrefix => true;
		public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
		{
			var propName = keyWord.Substring(KeyWord.Length);

			var otherArr = obj[propName] as JArray;


			var arr = jOptions as JArray;

			if (arr == null)
				throw new Exception();

			if (otherArr == null)
			{
				obj[propName] = jOptions;
			}
			else
			{
				foreach (var item in arr)
				{
					if (!otherArr.Children().Any(item2 => JToken.DeepEquals(item2, item)))
						otherArr.Add(item);
				}
			}

			return obj;
		}
	}
}
