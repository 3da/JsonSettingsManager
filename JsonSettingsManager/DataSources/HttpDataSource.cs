using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
    public class HttpDataSource : IDataSource
    {
        public string Uri { get; set; }
        public (JToken, IDataSource) Load(IDataSource lastDataSource, LoadMode mode, ParseContext context)
        {
            if (mode != LoadMode.Json)
                throw new NotImplementedException();

            using (var client = new HttpClient())
            {
                var str = client.GetStringAsync(Uri).Result;

                return (JToken.Parse(str), this);
            }
        }
    }
}
