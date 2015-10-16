using System;
using Noobot.Domain.DependencyResolution;
using Noobot.Domain.Plugins;
using Noobot.Domain.Slack;

namespace Noobot.Runner
{
    public class NoobotHost : INoobotHost
    {
        private readonly IContainerGenerator _containerGenerator;
        private ISlackWrapper _slackWrapper = null;
        private IPlugin[] _plugins = new IPlugin[0];

        public NoobotHost(IContainerGenerator containerGenerator)
        {
            _containerGenerator = containerGenerator;
        }

        public void Start()
        {
            INoobotContainer container = _containerGenerator.Generate();
            _slackWrapper = container.GetSlackConnector();

            Console.WriteLine("Connecting...");
            _slackWrapper
                .Connect()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        Console.WriteLine("CONNECTED :-)");

                        _plugins = container.GetPlugins();
                        foreach (IPlugin plugin in _plugins)
                        {
                            plugin.Start();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error connecting to Slack: {0}", task.Exception);
                    }
                });
        }

        public void Stop()
        {
            Console.WriteLine("Disconnecting...");
            _slackWrapper.Disconnect();
            Console.WriteLine("DISCONNECTED :(");

            foreach (IPlugin plugin in _plugins)
            {
                plugin.Stop();
            }
        }
    }
}