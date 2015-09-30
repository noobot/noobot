using System;
using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class BeginMessageMiddleware : MiddlewareBase
    {
        public BeginMessageMiddleware(IMiddleware next) : base(next)
        { }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            Console.WriteLine("[{0}] Message from {1}: {2}", message.MessageId, message.Username, message.Text);
            return Next(message);
        }
    }
}