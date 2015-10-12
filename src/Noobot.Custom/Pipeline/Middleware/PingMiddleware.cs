using System.Collections.Generic;
using Noobot.Custom.Plugins;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class PingMiddleware : MiddlewareBase
    {
        private readonly PingPlugin _pingPlugin;

        public PingMiddleware(IMiddleware next, PingPlugin pingPlugin) : base(next)
        {
            _pingPlugin = pingPlugin;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{ "ping me" },
                    Description = "Sends you a ping about every second",
                    EvaluatorFunc = PingHandler
                },
                new HandlerMapping
                {
                    ValidHandles = new []{ "stop pinging me" },
                    Description = "Stops sending you pings",
                    EvaluatorFunc = StopPingingHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> PingHandler(IncomingMessage message)
        {
            yield return message.ReplyToChannel("Ok, I will start pinging @" + message.Username);
            _pingPlugin.PingUserId(message.UserId);
        }

        private IEnumerable<ResponseMessage> StopPingingHandler(IncomingMessage message)
        {
            if (_pingPlugin.StopPingingUser(message.UserId))
            {
                yield return message.ReplyToChannel("Ok, I will stop pinging @" + message.Username);
            }
            else
            {
                yield return message.ReplyToChannel("BUT I AM NOT PINGING @" + message.Username);
            }
        }
    }
}