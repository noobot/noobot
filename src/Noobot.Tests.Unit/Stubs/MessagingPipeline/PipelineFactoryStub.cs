using Noobot.Core.DependencyResolution;
using Noobot.Core.MessagingPipeline;
using Noobot.Core.MessagingPipeline.Middleware;

namespace Noobot.Tests.Unit.Stubs.MessagingPipeline
{
    public class PipelineFactoryStub : IPipelineFactory
    {
        public void SetContainer(INoobotContainer noobotContainer)
        {
            throw new System.NotImplementedException();
        }

        public IMiddleware GetPipeline()
        {
            throw new System.NotImplementedException();
        }
    }
}