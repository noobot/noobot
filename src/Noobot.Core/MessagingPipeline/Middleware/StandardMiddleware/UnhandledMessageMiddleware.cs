using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    /// <summary>
    /// Should always be the last middleware. Simply logs and stops the chain.
    /// </summary>
    internal class UnhandledMessageMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public UnhandledMessageMiddleware(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            _logger.LogInformation("Unhandled message.");

            if(message.ChannelType == ResponseType.DirectMessage)
            {
                yield return message.ReplyToChannel("Sorry, I didn't understand that request.");
                yield return message.ReplyToChannel("Just type `help` so I can show you what I can do!");
            }
        }

        public IEnumerable<CommandDescription> GetSupportedCommands() => new CommandDescription[0];
    }
}