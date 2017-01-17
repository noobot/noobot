using System;
using Common.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;

namespace Noobot.Runner
{
    /// <summary>
    /// NoobotHost is required due to TopShelf.
    /// </summary>
    public class NoobotHost
    {
        private readonly IConfigReader _configReader;
        private readonly ILog _logger;
        private INoobotCore _noobotCore;

        public NoobotHost(IConfigReader configReader)
        {
            _configReader = configReader;
            _logger = LogManager.GetLogger(GetType());
        }

        public void Start()
        {
            IContainerFactory containerFactory = new ContainerFactory(new ConfigurationBase(), _configReader, _logger);
            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            Console.WriteLine("Connecting...");
            _noobotCore
                .Connect()
                .ContinueWith(task =>
                {
                    if (!task.IsCompleted || task.IsFaulted)
                    {
                        Console.WriteLine($"Error connecting to Slack: {task.Exception}");
                    }
                });
        }

        public void Stop()
        {
            Console.WriteLine("Disconnecting...");
            _noobotCore.Disconnect();
        }
    }
}