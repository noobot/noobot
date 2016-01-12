using System;
using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class StatsMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public StatsMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{ "stats" },
                    Description = "Returns interesting stats about your noobot installation",
                    EvaluatorFunc = StatsHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> StatsHandler(IncomingMessage message, string matchedHandle)
        {
            string textMessage = string.Join(Environment.NewLine, _statsPlugin.GetStats());

            if (!string.IsNullOrEmpty(textMessage))
            {
                yield return message.ReplyToChannel(">>>" + textMessage);
            }
            else
            {
                yield return message.ReplyToChannel("No stats have been recorded yet.");
            }
        }
    }
}