using Noobot.Core.MessagingPipeline;
using Noobot.Custom.Pipeline.Middleware;

namespace Noobot.Custom
{
    public class PipelineConfiguration : PipelineConfigurationBase
    {
        public PipelineConfiguration()
        {
            //Use<AutoResponderMiddleware>();
            Use<WelcomeMiddleware>();
            Use<AdminMiddleware>();
            Use<ScheduleMiddleware>();
            Use<JokeMiddleware>();
            Use<YieldTestMiddleware>();
            Use<PingMiddleware>();
            Use<FlickrMiddleware>();
            Use<CalculatorMiddleware>();
        }
    }
}