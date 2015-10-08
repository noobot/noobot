using Noobot.Domain.Plugins;
using Noobot.Domain.Plugins.StandardPlugins;

namespace Noobot.Runner
{
    public class PluginManager : PluginManagerBase
    {
        protected override void Initialise()
        {
            Use<PingPlugin>();
        }
    }
}