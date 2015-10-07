using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware;
using Registry = StructureMap.Configuration.DSL.Registry;

namespace Noobot.Tests.Unit.Stubs.MessagingPipeline
{
    public class PipelineManagerStub : IPipelineManager
    {
        public Registry Initialise(Registry registry)
        {
            throw new System.NotImplementedException();
        }

        public IMiddleware GetPipeline()
        {
            return new UnhandledMessageMiddleware();
        }
    }
}