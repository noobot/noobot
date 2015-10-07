using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
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
            yield return message.ReplyToChannel(string.Format("Hey @{0}, how you doing?", message.Username));
            Thread.Sleep(TimeSpan.FromSeconds(5));
            yield return message.ReplyDirectlyToUser("I know where you live...");
        }
    }
}