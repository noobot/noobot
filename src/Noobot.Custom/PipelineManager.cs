using Noobot.Core.MessagingPipeline;
using Noobot.Custom.Pipeline.Middleware;

namespace Noobot.Custom
{
    public class PipelineManager : PipelineManagerBase
    {
        protected override void Initialise()
        {
            //Use<AutoResponderMiddleware>();
            Use<WelcomeMiddleware>();
            Use<JokeMiddleware>();
            Use<YieldTestMiddleware>();
            Use<PingMiddleware>();
            Use<FlickrMiddleware>();
            Use<CalculatorMiddleware>();
        }
    }
}