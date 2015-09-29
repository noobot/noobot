using System;
using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline.Middlewares
{
    public class BeginMessageMiddleware : MiddlewareBase
    {
        public BeginMessageMiddleware(IMiddleware next) : base(next)
        { }

        public override async Task<Response> Invoke(IncomingMessage message)
        {
            Console.WriteLine("[{0}] Message from {1}: {2}", message.MessageId, message.Username, message.Text);
            return await Next(message);
        }
    }
}