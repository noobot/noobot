using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline
{
    public abstract class MiddlewareBase
    {
        private readonly MiddlewareBase _next;

        protected MiddlewareBase(MiddlewareBase next)
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