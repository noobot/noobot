using System;
using System.Threading.Tasks;
using Noobot.Domain.Configuration;
using Noobot.Domain.MessagingPipeline;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using Response = Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.Slack
{
    public class SlackConnector : ISlackConnector
    {
        private readonly IConfigReader _configReader;
        private readonly IPipelineManager _pipelineManager;
        private SlackSocketClient _client;

        public SlackConnector(IConfigReader configReader, IPipelineManager pipelineManager)
        {
            _configReader = configReader;
            _pipelineManager = pipelineManager;
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
                _pipelineManager.Initialise();

                _client.OnMessageReceived += ClientOnOnMessageReceived;

                var connectionStatus = new InitialConnectionStatus();
                tcs.SetResult(connectionStatus);
            });

            return await tcs.Task;
        }

        private void ClientOnOnMessageReceived(NewMessage newMessage)
        {
            Console.WriteLine("[[[Message started]]]");

            IMiddleware pipeline = _pipelineManager.GetPipeline();
            var incomingMessage = new IncomingMessage
            {
                MessageId = newMessage.id,
                Text = newMessage.text,
                UserId = newMessage.user,
                Channel = newMessage.channel
            };

            Task<Response> messageTask = pipeline.Invoke(incomingMessage);
            messageTask.ContinueWith(task =>
            {
                // message completed. Send messages etc here
                Console.WriteLine("[[[Message ended]]]");
            });
        }

        public void Disconnect()
        {
            if (_client != null && _client.IsConnected)
            {
                _client.CloseSocket();
            }
        }
    }
}