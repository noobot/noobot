using System;
using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Plugins.StandardPlugins;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
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
            _statsPlugin.RecordStat("MessagesReceived", 1);
            Console.WriteLine("Message from {0}: {1}",  message.Username, message.FullText);
            return Next(message);
        }
    }
}