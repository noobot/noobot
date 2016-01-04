using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class HelpMiddleware : MiddlewareBase
    {
        public HelpMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new[] {"help", "yo tell me more"},
                    Description = "Returns supported commands and descriptions of how to use them",
                    EvaluatorFunc = HelpHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> HelpHandler(IncomingMessage message, string matchedHandle)
        {
            var builder = new StringBuilder();
            builder.Append(">>>");

            var supportedCommands = GetSupportedCommands().OrderBy(x => x.Command);

            foreach (CommandDescription commandDescription in supportedCommands)
            {
                builder.AppendFormat("{0}\t- {1}\n", commandDescription.Command, commandDescription.Description);
            }

            yield return message.ReplyDirectlyToUser(builder.ToString());
        }
    }
}