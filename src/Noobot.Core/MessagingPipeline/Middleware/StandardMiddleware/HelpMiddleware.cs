using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class HelpMiddleware : MiddlewareBase
    {
        private readonly INoobotCore _noobotCore;

        public HelpMiddleware(IMiddleware next, INoobotCore noobotCore) : base(next)
        {
            _noobotCore = noobotCore;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []
                    {
                        new ValidHandle
                        {
                            MatchType = ValidHandle.ValidHandleMatchType.StartsWith,
                            MatchText = "help"
                        },
                        new ValidHandle
                        {
                            MatchType = ValidHandle.ValidHandleMatchType.ExactMatch,
                            MatchText = "yo tell me more"
                        }
                    },
                    Description = "Returns supported commands and descriptions of how to use them",
                    EvaluatorFunc = HelpHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> HelpHandler(IncomingMessage message, ValidHandle matchedHandle)
        {
            var builder = new StringBuilder();
            builder.Append(">>>");

            IEnumerable<CommandDescription> supportedCommands = GetSupportedCommands().OrderBy(x => x.Command);

            foreach (CommandDescription commandDescription in supportedCommands)
            {
                string description = commandDescription.Description.Replace("@{bot}", $"@{_noobotCore.GetBotUserName()}");
                builder.AppendFormat("{0}\t- {1}\n", commandDescription.Command, description);
            }

            yield return message.ReplyToChannel(builder.ToString());
        }
    }
}
