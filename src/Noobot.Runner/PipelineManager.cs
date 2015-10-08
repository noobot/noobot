using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware;

namespace Noobot.Runner
{
    public class PipelineManager : PipelineManagerBase
    {
        protected override void Initialise()
        {
            Use<BeginMessageMiddleware>();
            Use<HelpMiddleware>();
            Use<WelcomeMiddleware>();
            Use<JokeMiddleware>();
            Use<AboutMiddleware>();
            Use<YieldTestMiddleware>();
            Use<PingMiddleware>();
            Use<UnhandledMessageMiddleware>();
        }
    }
}