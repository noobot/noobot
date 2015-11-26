using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Plugins.StandardPlugins;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class WelcomeMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public WelcomeMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{"hi", "hey", "hello", "wuzzup"},
                    Description = "Try saying hi and see what happens",
                    EvaluatorFunc = WelcomeHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> WelcomeHandler(IncomingMessage message, string matchedHandle)
        {
            _statsPlugin.IncrementState("Hello");

            yield return message.ReplyToChannel($"Hey @{message.Username}, how you doing?");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            yield return message.ReplyDirectlyToUser("I know where you live...");
        }
    }
}