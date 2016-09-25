using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.Plugins;
using StructureMap;

namespace Noobot.Core.DependencyResolution
{
    public interface INoobotContainer
    {
        INoobotCore GetNoobotCore();
        IPlugin[] GetPlugins();
        T GetPlugin<T>() where T : class, IPlugin;
        IMiddleware GetMiddlewarePipeline();
        IContainer GetStructuremapContainer();
    }
}