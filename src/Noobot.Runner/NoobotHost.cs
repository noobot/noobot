using System;
using Noobot.Core.DependencyResolution;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using Noobot.Core.Slack;

namespace Noobot.Runner
{
    public class NoobotHost : INoobotHost
    {
        private readonly IContainerFactory _containerFactory;
        private INoobotCore _noobotCore;
        private IPlugin[] _plugins = new IPlugin[0];

        public NoobotHost(IContainerFactory containerFactory)
        {
            _containerFactory = containerFactory;
        }

        public void Start()
        {
            INoobotContainer container = _containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            Console.WriteLine("Connecting...");
            _noobotCore
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
            _noobotCore.Disconnect();

            foreach (IPlugin plugin in _plugins)
            {
                plugin.Stop();
            }
        }
    }
}