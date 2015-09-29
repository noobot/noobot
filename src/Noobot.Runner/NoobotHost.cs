using System;
using Noobot.Domain.Slack;

namespace Noobot.Runner
{
    public class NoobotHost : INoobotHost
    {
        private readonly ISlackConnector _slackConnector;

        public NoobotHost(ISlackConnector slackConnector)
        {
            _slackConnector = slackConnector;
        }

        public void Start()
        {
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