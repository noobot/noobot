using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middlewares;

namespace Noobot.Runner
{
    public class PipelineManager : PipelineManagerBase
    {
        public override void Initialise()
        {
            Use<ErrorHandlerMiddleware>();
            Use<BeginMessageMiddleware>();
            Use<TestMiddleware>();
            Use<UnhandledMessageMiddleware>();
        }
    }
}