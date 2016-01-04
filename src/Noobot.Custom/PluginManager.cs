using Noobot.Core.Plugins;
using Noobot.Custom.Plugins;

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