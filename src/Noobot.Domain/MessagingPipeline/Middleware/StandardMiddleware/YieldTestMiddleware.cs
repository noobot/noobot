using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class YieldTestMiddleware : MiddlewareBase
    {
        public YieldTestMiddleware(IMiddleware next) : base(next)
        { }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Sending message");
                yield return new ResponseMessage { Channel = message.Channel, Text = "Waiting " + i };
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            
            foreach (ResponseMessage responseMessage in Next(message))
            {
                yield return responseMessage;
            }
        }
    }
}