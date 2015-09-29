using Noobot.Domain.Configuration;

namespace Noobot.Domain.Slack
{
    public class SlackConnector : ISlackConnector
    {
        private readonly IConfigReader _configReader;

        public SlackConnector(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public InitialConnectionStatus Connect()
        {
            throw new System.NotImplementedException();
        }
    }
}