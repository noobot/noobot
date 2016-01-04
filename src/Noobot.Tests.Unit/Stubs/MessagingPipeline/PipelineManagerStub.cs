using Noobot.Core.MessagingPipeline;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware;
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