using Newtonsoft.Json.Linq;

namespace Noobot.Domain.Configuration
{
    public interface IConfigReader
    {
        JObject GetConfig();
    }
}