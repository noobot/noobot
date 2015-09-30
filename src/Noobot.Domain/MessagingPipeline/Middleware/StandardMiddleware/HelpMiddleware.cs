using System.Collections.Generic;
using System.Text;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class HelpMiddleware : MiddlewareBase
    {
        public HelpMiddleware(IMiddleware next) : base(next)
        { }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            if (message.Text.Equals("help"))
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
            return new []
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