using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class AutoResponderMiddleware : MiddlewareBase
    {
        public AutoResponderMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new []
            {
                new HandlerMapping
                {
                    ValidHandles = new [] { ""},
                    Description = "Annoys the heck out of everyone",
                    EvaluatorFunc = AutoResponseHandler,
                    FilterMessagesDirectedAtBot = false,
                    ShouldContinueProcessing = true
                }
            };
        }

        private IEnumerable<ResponseMessage> AutoResponseHandler(IncomingMessage message, string matchedHandle)
        {
            yield return message.ReplyDirectlyToUser(message.FullText);
        }
    }
}