using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class BeginMessageMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;
        private readonly ILogger _logger;

        public BeginMessageMiddleware(IMiddleware next, StatsPlugin statsPlugin, ILogger logger) : base(next)
        {
            _statsPlugin = statsPlugin;
            _logger = logger;
        }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            _statsPlugin.IncrementState("Messages:Received");
            _logger.LogInformation($"Message from {message.Username}: {message.FullText}");

            foreach (ResponseMessage responseMessage in Next(message))
            {
                _statsPlugin.IncrementState("Messages:Sent");
                yield return responseMessage;
            }
        }
    }
}