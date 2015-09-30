using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.StandardMiddleware;

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