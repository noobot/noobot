using Noobot.Domain.Configuration;
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
            
        }

        public void Stop()
        {
            
        }
    }
}