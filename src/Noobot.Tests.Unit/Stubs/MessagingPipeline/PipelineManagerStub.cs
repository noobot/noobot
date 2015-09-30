using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware;

namespace Noobot.Tests.Unit.Stubs.MessagingPipeline
{
    public class PipelineManagerStub : IPipelineManager
    {
        public void Initialise()
        {
            
        }

        public IMiddleware GetPipeline()
        {
            return new UnhandledMessageMiddleware();
        }
    }
}