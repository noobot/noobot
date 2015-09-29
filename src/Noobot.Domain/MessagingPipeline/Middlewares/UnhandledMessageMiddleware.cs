using System;
using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline.Middlewares
{
    public class UnhandledMessageMiddleware : MiddlewareBase
    {
        public UnhandledMessageMiddleware(MiddlewareBase next) : base(next)
        { }

        public override Task<Response> Invoke(IncomingMessage message)
        {
            Console.WriteLine("[{0}] Unhandled", message.MessageId);
            var emptyResponse = new Response();
            return Task.FromResult(emptyResponse);
        }
    }
}