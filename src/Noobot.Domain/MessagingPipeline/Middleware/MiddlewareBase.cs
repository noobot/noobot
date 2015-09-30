using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware
{
    public abstract class MiddlewareBase : IMiddleware
    {
        private readonly IMiddleware _next;

        protected MiddlewareBase(IMiddleware next)
        {
            _next = next;
        }

        protected async Task<MiddlewareResponse> Next(IncomingMessage message)
        {
            return await _next.Invoke(message);
        }

        public abstract Task<MiddlewareResponse> Invoke(IncomingMessage message);
    }
}