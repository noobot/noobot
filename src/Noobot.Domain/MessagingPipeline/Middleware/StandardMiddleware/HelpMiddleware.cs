using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class HelpMiddleware : MiddlewareBase
    {
        private HandlerMapping[] _handlerMappings;

        public HelpMiddleware(IMiddleware next) : base(next)
        {
            _handlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new [] {"help", "yo tell me"},
                    Description = "Returns supported commands and descriptions of how to use them",
                    EvaluatorFunc = HelpHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> HelpHandler(IncomingMessage message)
        {
            return new ResponseMessage[0];
        }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {


            var p = new OptionSet
            {
                {"h|help", s =>
                    {
                        var builder = new StringBuilder();
                        builder.Append(">>>");

                        foreach (var commandDescription in GetSupportedCommands())
                        {
                            builder.AppendFormat("{0}\t- {1}\n", commandDescription.Command, commandDescription.Description);
                        }

                        yield return message.ReplyDirectlyToUser(builder.ToString());
                    }
                }
            };




            if (message.Text.Equals("help", StringComparison.InvariantCultureIgnoreCase))
            {
                var builder = new StringBuilder();
                builder.Append(">>>");

                foreach (var commandDescription in GetSupportedCommands())
                {
                    builder.AppendFormat("{0}\t- {1}\n", commandDescription.Command, commandDescription.Description);
                }

                yield return message.ReplyDirectlyToUser(builder.ToString());
            }
            else
            {
                foreach (ResponseMessage responseMessage in Next(message))
                {
                    yield return responseMessage;
                }
            }
        }

        protected override CommandDescription[] SupportedCommands()
        {
            return new[]
            {
                new CommandDescription
                {
                    Command = "help",
                    Description = "Returns supported commands and descriptions of how to use them"
                }
            };
        }
    }
}