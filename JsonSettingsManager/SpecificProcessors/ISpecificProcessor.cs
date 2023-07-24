using System.Threading;
using System.Threading.Tasks;
using JsonSettingsManager.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.SpecificProcessors
{
    public interface ISpecificProcessor
    {
        string KeyWord { get; }

        bool IsPrefix { get; }

        Task<JToken> DoAsync(ParseContext context, JToken jOptions, JObject obj, string keyWord, CancellationToken token = default);
    }
}