using System;
using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline.StandardMiddleware
{
    /// <summary>
    /// Should always be the last middleware. Simply logs and stops the chain.
    /// </summary>
    public class UnhandledMessageMiddleware : IMiddleware
    {
        public Task<Response> Invoke(IncomingMessage message)
        {
            Console.WriteLine("[{0}] Unhandled", message.MessageId);
            return Task.FromResult<Response>(null);
        }
    }
}