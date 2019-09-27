using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
    internal class ExceptArrayProcessor : ISpecificProcessor
    {
        public string KeyWord => "except-";
        public bool IsPrefix => true;
        public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
        {
            var arr = jOptions as JArray;

            if (arr == null)
                throw new Exception();

            var propName = keyWord.Substring(KeyWord.Length);

            var otherArr = obj[propName] as JArray;


            if (otherArr == null)
                return obj;

            foreach (var item in arr)
            {
                var item2 = otherArr.Children().FirstOrDefault(q => JToken.DeepEquals(q, item));
                if (item2 != null)
                    otherArr.Remove(item2);
            }

            return obj;
        }
    }
}
