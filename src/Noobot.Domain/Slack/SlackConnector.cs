using System.Threading.Tasks;
using Noobot.Domain.Configuration;
using SlackAPI;

namespace Noobot.Domain.Slack
{
    public class SlackConnector : ISlackConnector
    {
        private readonly IConfigReader _configReader;
        private SlackSocketClient _client;

        public SlackConnector(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public async Task<InitialConnectionStatus> Connect()
        {
            var tcs = new TaskCompletionSource<InitialConnectionStatus>();

            var config = _configReader.GetConfig();
            _client = new SlackSocketClient(config.Slack.ApiToken);

            LoginResponse loginResponse;
            _client.Connect(response =>
            {
                //This is called once the client has emitted the RTM start command
                loginResponse = response;
            }, 
            () =>
            {
                //This is called once the Real Time Messaging client has connected to the end point
                var connectionStatus = new InitialConnectionStatus();
                tcs.SetResult(connectionStatus);
            });

            return await tcs.Task;
        }
    }
}