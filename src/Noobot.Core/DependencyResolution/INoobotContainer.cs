using Noobot.Core.Plugins;

namespace Noobot.Core.DependencyResolution
{
    public interface INoobotContainer : StructureMap.IContainer
    {
        INoobotCore GetNoobotCore();
        IPlugin[] GetPlugins();
    }
}