using System;
using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class BeginMessageMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public BeginMessageMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
        }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            _statsPlugin.IncrementState("Messages:Received");
            Console.WriteLine("Message from {0}: {1}",  message.Username, message.FullText);

            foreach (ResponseMessage responseMessage in Next(message))
            {
                _statsPlugin.IncrementState("Messages:Sent");
                yield return responseMessage;
            }
        }
    }
}