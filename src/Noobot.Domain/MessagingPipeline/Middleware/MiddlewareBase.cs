using System.Collections.Generic;
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

        protected IEnumerable<ResponseMessage> Next(IncomingMessage message)
        {
            return _next.Invoke(message);
        }

        public abstract IEnumerable<ResponseMessage> Invoke(IncomingMessage message);

        public virtual IEnumerable<CommandDescription> GetSupportedCommands()
        {
            return _next.GetSupportedCommands();
        }
    }
}