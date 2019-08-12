using System;
using System.Collections.Generic;
using System.Text;
using JsonSettingsManager.DataSources;
using Newtonsoft.Json;

namespace JsonSettingsManager
{
	internal class ParseContext
	{
		public IDataSource DataSource { get; set; }
		public bool MergeArray { get; set; }
		public SettingsManager Manager { get; set; }
		public JsonSerializer Serializer { get; set; }
	}
}
