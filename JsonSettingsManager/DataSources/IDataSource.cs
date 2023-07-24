using System;
using System.Threading;
using System.Threading.Tasks;
using JsonSettingsManager.TypeResolving;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
    [ResolveType]
    public interface IDataSource
    {
        Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context, CancellationToken token = default);
    }
}