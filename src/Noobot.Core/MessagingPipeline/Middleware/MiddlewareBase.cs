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

        protected MiddlewareBase()
        {
            HandlerMappings = HandlerMappings ?? new HandlerMapping[0];
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
                            yield return MiddlewareSingals.StopProcessing;
                            // TODO: Assert.Fail() here?
                        }
                    }
                }
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
        }
    }
}