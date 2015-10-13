using Noobot.Custom.Pipeline.Middleware;
using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware;

namespace Noobot.Custom
{
    public class PipelineManager : PipelineManagerBase
    {
        protected override void Initialise()
        {
            Use<WelcomeMiddleware>();
            Use<JokeMiddleware>();
            Use<AboutMiddleware>();
            Use<YieldTestMiddleware>();
            Use<PingMiddleware>();
            Use<FlickrMiddleware>();
        }
    }
}