using System;
using System.Collections.Generic;
using Noobot.Core.Logging;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class BeginMessageMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;
        private readonly ILog _log;

        public BeginMessageMiddleware(IMiddleware next, StatsPlugin statsPlugin, ILog log) : base(next)
        {
            _statsPlugin = statsPlugin;
            _log = log;
        }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            _statsPlugin.IncrementState("Messages:Received");
            _log.Log($"Message from {message.Username}: {message.FullText}");

            foreach (ResponseMessage responseMessage in Next(message))
            {
                _statsPlugin.IncrementState("Messages:Sent");
                yield return responseMessage;
            }
        }
    }
}