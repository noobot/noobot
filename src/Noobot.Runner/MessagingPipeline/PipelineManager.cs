using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middlewares;

namespace Noobot.Runner.MessagingPipeline
{
    public class PipelineManager : PipelineManagerBase
    {
        public override void Initialise()
        {
            Use<BeginMessageMiddleware>();
            Use<TestMiddleware>();
            Use<UnhandledMessageMiddleware>();
        }
    }
}