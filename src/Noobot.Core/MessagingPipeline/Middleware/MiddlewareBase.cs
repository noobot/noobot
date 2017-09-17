using System.Collections.Generic;
using System.Linq;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.MessagingPipeline.Middleware
{
    public abstract class MiddlewareBase : IMiddleware
    {
        protected HandlerMapping[] HandlerMappings;
        private readonly IMiddleware _next;

        protected MiddlewareBase(IMiddleware next)
        {
            _next = next;
            HandlerMappings = HandlerMappings ?? new HandlerMapping[0];
        }

        protected internal IEnumerable<ResponseMessage> Next(IncomingMessage message)
        {
            return _next?.Invoke(message) ?? new ResponseMessage[0];
        }

        public virtual IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            foreach (var handlerMapping in HandlerMappings)
            {
                foreach (IValidHandle handle in handlerMapping.ValidHandles)
                {
                    string messageText = message.FullText;
                    if (handlerMapping.MessageShouldTargetBot)
                    {
                        messageText = message.TargetedText;
                    }
                    
                    if (handle.IsMatch(messageText))
                    {
                        foreach (var responseMessage in handlerMapping.EvaluatorFunc(message, handle))
                        {
                            yield return responseMessage;
                        }

                        if (!handlerMapping.ShouldContinueProcessing)
                        {
                            yield break;
                        }
                    }
                }
            }

            foreach (ResponseMessage responseMessage in Next(message))
            {
                yield return responseMessage;
            }
        }

        public IEnumerable<CommandDescription> GetSupportedCommands()
        {
            foreach (var handlerMapping in HandlerMappings)
            {
                if (!handlerMapping.VisibleInHelp)
                {
                    continue;
                }

                yield return new CommandDescription
                {
                    Command = string.Join(" | ", handlerMapping.ValidHandles.Select(x => $"`{x.HandleHelpText}`").OrderBy(x => x)),
                    Description = handlerMapping.Description
                };
            }

            foreach (var commandDescription in _next.GetSupportedCommands())
            {
                yield return commandDescription;
            }
        }
    }
}