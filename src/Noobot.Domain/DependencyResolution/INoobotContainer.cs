using Noobot.Domain.Plugins;
using Noobot.Domain.Slack;

namespace Noobot.Domain.DependencyResolution
{
    public interface INoobotContainer : StructureMap.IContainer
    {
        ISlackWrapper GetSlackConnector();
        IPlugin[] GetPlugins();
    }
}