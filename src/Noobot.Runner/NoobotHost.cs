using System;
using Noobot.Domain.DependencyResolution;
using Noobot.Domain.Slack;

namespace Noobot.Runner
{
    public class NoobotHost : INoobotHost
    {
        private readonly IContainerGenerator _containerGenerator;
        private ISlackConnector _slackConnector = null;

        public NoobotHost(IContainerGenerator containerGenerator)
        {
            _containerGenerator = containerGenerator;
        }

        public void Start()
        {
            INoobotContainer container = _containerGenerator.Generate();
            _slackConnector = container.GetSlackConnector();

            Console.WriteLine("Connecting...");
            _slackConnector.Connect().Wait();
            Console.WriteLine("CONNECTED :-)");
        }

        public void Stop()
        {
            Console.WriteLine("Disconnecting...");
            _slackConnector.Disconnect();
            Console.WriteLine("DISCONNECTED :(");
        }
    }
}