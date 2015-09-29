using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline
{
    public abstract class MiddlewareBase : IMiddleware
    {
        private readonly IMiddleware _next;

        protected MiddlewareBase(IMiddleware next)
        {
            _next = next;
        }

        protected async Task<Response> Next(IncomingMessage message)
        {
            return await _next.Invoke(message);
        }

        public abstract Task<Response> Invoke(IncomingMessage message);
    }
}