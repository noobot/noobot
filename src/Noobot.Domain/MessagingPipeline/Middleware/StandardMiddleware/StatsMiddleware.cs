using System;
using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Plugins.StandardPlugins;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
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
                    ValidHandles = new []{ "/stats", "stats" },
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