using Noobot.Domain.DependencyResolution;
using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middleware;

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