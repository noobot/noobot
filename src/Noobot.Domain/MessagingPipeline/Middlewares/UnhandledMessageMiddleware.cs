using System;
using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline.Middlewares
{
    /// <summary>
    /// Should always be the last middleware. Simply logs and stops the chain.
    /// </summary>
    public class UnhandledMessageMiddleware : IMiddleware
    {
        public Task<Response> Invoke(IncomingMessage message)
        {
            Console.WriteLine("[{0}] Unhandled", message.MessageId);
            var emptyResponse = new Response();
            return Task.FromResult(emptyResponse);
        }
    }
}