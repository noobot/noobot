using Noobot.Core.Configuration;
using Noobot.Custom.Pipeline.Middleware;
using Noobot.Custom.Plugins;

namespace Noobot.Custom
{
    public class Configuration : ConfigurationBase
    {
        public Configuration()
        {
            //UseMiddleware<AutoResponderMiddleware>();
            UseMiddleware<WelcomeMiddleware>();
            UseMiddleware<AdminMiddleware>();
            UseMiddleware<ScheduleMiddleware>();
            UseMiddleware<JokeMiddleware>();
            UseMiddleware<YieldTestMiddleware>();
            UseMiddleware<PingMiddleware>();
            UseMiddleware<FlickrMiddleware>();
            UseMiddleware<CalculatorMiddleware>();
            
            UsePlugin<StoragePlugin>();
            UsePlugin<SchedulePlugin>();
            UsePlugin<AdminPlugin>();
            UsePlugin<PingPlugin>();
        }
    }
}