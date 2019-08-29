using System;
using JsonSettingsManager.TypeResolving;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
	[ResolveType]
	public interface IDataSource
	{
		 (JToken, IDataSource) Load(IDataSource lastDataSource, LoadMode mode, ParseContext context);
	}
}