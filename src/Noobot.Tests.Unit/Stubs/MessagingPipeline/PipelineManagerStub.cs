using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middlewares;

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