using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
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
                    ValidHandles = new IValidHandle[]
                    {
                        new StartsWithHandle("help"),
                        new ExactMatchHandle("yo tell me more")
                    },
                    Description = "Returns supported commands and descriptions of how to use them",
                    EvaluatorFunc = HelpHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> HelpHandler(IncomingMessage message, IValidHandle matchedHandle)
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
