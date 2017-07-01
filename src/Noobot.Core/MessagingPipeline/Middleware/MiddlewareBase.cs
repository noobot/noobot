using System;
using System.Collections.Generic;
using System.Linq;
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

        protected IEnumerable<ResponseMessage> Next(IncomingMessage message)
        {
            return _next.Invoke(message);
        }

        public virtual IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            foreach (var handlerMapping in HandlerMappings)
            {
                foreach (ValidHandle map in handlerMapping.ValidHandles)
                {

                    //check the handler type, and then match the text in the appropriate fashion
                    string messageText = message.FullText;
                    if (handlerMapping.MessageShouldTargetBot)
                    {
                        messageText = message.TargetedText;
                    }

                    //check the handler type, and then match the text in the appropriate fashion
                    bool isMatch = false;
                    switch (map.MatchType) {
                        case ValidHandle.ValidHandleMatchType.ProcessAll:
                            isMatch = true;
                            break;
                        case ValidHandle.ValidHandleMatchType.ExactMatch:
                            isMatch = messageText.Equals(map.MatchText, StringComparison.InvariantCultureIgnoreCase);
                            break;
                        case ValidHandle.ValidHandleMatchType.StartsWith:
                            isMatch = messageText.StartsWith(map.MatchText, StringComparison.InvariantCultureIgnoreCase);
                            break;
                        case ValidHandle.ValidHandleMatchType.Contains:
                            isMatch = messageText.IndexOf(map.MatchText, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            break;
                        case ValidHandle.ValidHandleMatchType.RegEx:
                            isMatch = System.Text.RegularExpressions.Regex.IsMatch(messageText, map.MatchText, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            break;
                    }


                    if (isMatch)
                    {
                        //TODO: How to do this
                        //_log.Log($"Matched '{map}' on '{this.GetType().Name}'");

                        foreach (var responseMessage in handlerMapping.EvaluatorFunc(message, map))
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
                    Command = string.Join(" | ", handlerMapping.ValidHandles.Select(x => $"`{x}`").OrderBy(x => x)),
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