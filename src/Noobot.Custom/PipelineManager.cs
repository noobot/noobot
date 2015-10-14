using Noobot.Custom.Pipeline.Middleware;
using Noobot.Domain.MessagingPipeline;

namespace Noobot.Custom
{
    public class PipelineManager : PipelineManagerBase
    {
        protected override void Initialise()
        {
            Use<WelcomeMiddleware>();
            Use<JokeMiddleware>();
            Use<YieldTestMiddleware>();
            Use<PingMiddleware>();
            Use<FlickrMiddleware>();
        }
    }
}