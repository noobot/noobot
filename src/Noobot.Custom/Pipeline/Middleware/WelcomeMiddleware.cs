using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class WelcomeMiddleware : MiddlewareBase
    {
        public WelcomeMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{"hi", "hey", "hello", "wuzzup"},
                    Description = "Try saying hi and see what happens",
                    EvaluatorFunc = TestHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> TestHandler(IncomingMessage message)
        {
            yield return message.ReplyToChannel($"Hey @{message.Username}, how you doing?");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            yield return message.ReplyDirectlyToUser("I know where you live...");
        }
    }
}