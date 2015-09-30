using System;
using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class BeginMessageMiddleware : MiddlewareBase
    {
        public BeginMessageMiddleware(IMiddleware next) : base(next)
        { }

        public override async Task<MiddlewareResponse> Invoke(IncomingMessage message)
        {
            Console.WriteLine("[{0}] Message from {1}: {2}", message.MessageId, message.Username, message.Text);
            return await Next(message);
        }
    }
}