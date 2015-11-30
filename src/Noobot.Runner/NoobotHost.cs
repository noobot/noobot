using System;
using Noobot.Domain.DependencyResolution;
using Noobot.Domain.Plugins;
using Noobot.Domain.Plugins.StandardPlugins;
using Noobot.Domain.Slack;

namespace Noobot.Runner
{
    public class NoobotHost : INoobotHost
    {
        private readonly IContainerGenerator _containerGenerator;
        private ISlackWrapper _slackWrapper;
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
                        _plugins = container.GetPlugins();
                        foreach (IPlugin plugin in _plugins)
                        {
                            plugin.Start();
                        }

                        container.GetInstance<StatsPlugin>().RecordStat("Connected since", DateTime.Now.ToString("G"));
                    }
                    else
                    {
                        Console.WriteLine($"Error connecting to Slack: {task.Exception}");
                    }
                });
        }

        public void Stop()
        {
            Console.WriteLine("Disconnecting...");
            _slackWrapper.Disconnect();

            foreach (IPlugin plugin in _plugins)
            {
                plugin.Stop();
            }
        }
    }
}