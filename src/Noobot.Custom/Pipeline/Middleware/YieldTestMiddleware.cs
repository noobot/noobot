using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class YieldTestMiddleware : MiddlewareBase
    {
        public YieldTestMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new[] { "/yield", "yield test" },
                    EvaluatorFunc = YieldTest,
                    Description = "Just tests delayed messages"
                }
            };
        }

        private IEnumerable<ResponseMessage> YieldTest(IncomingMessage incomingMessage)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Sending message");
                yield return new ResponseMessage { Channel = incomingMessage.Channel, Text = "Waiting " + i };
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
    }
}