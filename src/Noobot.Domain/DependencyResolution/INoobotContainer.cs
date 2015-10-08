using Noobot.Domain.Plugins;
using Noobot.Domain.Slack;

namespace Noobot.Domain.DependencyResolution
{
    public interface INoobotContainer : StructureMap.IContainer
    {
        ISlackConnector GetSlackConnector();
        IPlugin[] GetPlugins();
    }
}