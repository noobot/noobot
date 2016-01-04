using Newtonsoft.Json.Linq;

namespace Noobot.Core.Configuration
{
    public interface IConfigReader
    {
        JObject GetConfig();
    }
}