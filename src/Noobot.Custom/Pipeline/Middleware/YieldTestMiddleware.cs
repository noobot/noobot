using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class YieldTestMiddleware : MiddlewareBase
    {
        public YieldTestMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new[] { "yield test" },
                    EvaluatorFunc = YieldTest,
                    Description = "Just tests delayed messages"
                }
            };
        }

        private IEnumerable<ResponseMessage> YieldTest(IncomingMessage incomingMessage, string matchedHandle)
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