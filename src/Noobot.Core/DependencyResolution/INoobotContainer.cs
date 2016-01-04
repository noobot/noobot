using Noobot.Core.Plugins;
using Noobot.Core.Slack;

namespace Noobot.Core.DependencyResolution
{
    public interface INoobotContainer : StructureMap.IContainer
    {
        INoobotCore GetNoobotCore();
        IPlugin[] GetPlugins();
    }
}