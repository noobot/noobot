using Noobot.Custom.Plugins;
using Noobot.Domain.Plugins;

namespace Noobot.Custom
{
    public class PluginManager : PluginManagerBase
    {
        protected override void Initialise()
        {
            Use<PingPlugin>();
        }
    }
}