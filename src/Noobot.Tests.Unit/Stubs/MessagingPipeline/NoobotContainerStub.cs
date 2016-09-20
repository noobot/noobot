using Noobot.Core;
using Noobot.Core.DependencyResolution;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.Plugins;

namespace Noobot.Tests.Unit.Stubs.MessagingPipeline
{
    public class NoobotContainerStub : INoobotContainer
    {
        public INoobotCore GetNoobotCore()
        {
            throw new System.NotImplementedException();
        }

        public IPlugin[] GetPlugins()
        {
            return new IPlugin[0];
        }

        public T GetPlugin<T>() where T : class, IPlugin
        {
            return null;
        }

        public IMiddleware GetMiddlewarePipeline()
        {
            return null;
        }
    }
}