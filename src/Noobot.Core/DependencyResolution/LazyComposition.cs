using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.Plugins.StandardPlugins;
using Common.Logging;

namespace Noobot.Core.DependencyResolution
{
    internal class LazyComposition
    {
        readonly Lazy<IMiddleware[]> middleware;
        readonly Lazy<IPlugin[]> plugins;
        readonly Lazy<ILog> logger;
        readonly Lazy<StatsPlugin> stats;
        readonly IServiceProvider container;

        public IPlugin[] GetPlugins()
        {
            return plugins.Value;
        }

        public IEnumerable<ResponseMessage> InvokePipeline(IncomingMessage message)
        {
            stats.Value.IncrementState("Messages:Received");
            logger.Value.Info($"Message from {message.Username}: {message.FullText}");

            foreach (var ware in middleware.Value)
            {
                foreach(var response in ware.Invoke(message))
                {
                    if(ReferenceEquals(MiddlewareSingals.StopProcessing, response))
                    {
                        yield break;
                    }
                    else
                    {
                        stats.Value.IncrementState("Messages:Sent");
                        yield return response;
                    }
                }
            }

            logger.Value.Info("Unhandled message.");

            if (message.ChannelType == ResponseType.DirectMessage)
            {
                yield return message.ReplyToChannel("Sorry, I didn't understand that request.");
                yield return message.ReplyToChannel("Just type `help` so I can show you what I can do!");
            }
        }

        public IEnumerable<CommandDescription> GetAllSupportedCommands()
        {
            foreach(var ware in middleware.Value)
            {
                foreach(var command in ware.GetSupportedCommands())
                {
                    yield return command;
                }
            }
        }

        public LazyComposition(IServiceProvider container)
        {
            this.container = container;
            middleware = new Lazy<IMiddleware[]>(() => this.container.GetServices<IMiddleware>().ToArray());
            plugins = new Lazy<IPlugin[]>(() => this.container.GetServices<IPlugin>().ToArray());
            logger = new Lazy<ILog>(container.GetService<ILog>);
            stats = new Lazy<StatsPlugin>(container.GetService<StatsPlugin>);
        }
    }
}