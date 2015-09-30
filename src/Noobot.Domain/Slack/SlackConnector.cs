using System;
using System.Threading.Tasks;
using Noobot.Domain.Configuration;
using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace Noobot.Domain.Slack
{
    public class SlackConnector : ISlackConnector
    {
        private readonly IConfigReader _configReader;
        private readonly IPipelineManager _pipelineManager;
        private SlackSocketClient _client;
        private string _myId;

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
                _myId = loginResponse.self.id;
            },
            () =>
            {
                //This is called once the Real Time Messaging client has connected to the end point
                _pipelineManager.Initialise();

                _client.OnMessageReceived += ClientOnOnMessageReceived;

                //TODO: Populate with useful information and display/log it somewhere
                var connectionStatus = new InitialConnectionStatus();
                tcs.SetResult(connectionStatus);
            });

            return await tcs.Task;
        }

        private void ClientOnOnMessageReceived(NewMessage newMessage)
        {
            // ignore self messages...you dirty thing
            if (newMessage.user == _myId)
            {
                return;
            }

            Console.WriteLine("[[[Message started]]]");

            IMiddleware pipeline = _pipelineManager.GetPipeline();
            var incomingMessage = new IncomingMessage
            {
                MessageId = newMessage.id,
                Text = newMessage.text,
                UserId = newMessage.user,
                Channel = newMessage.channel,
                Username = _client.UserLookup[newMessage.user].name
            };

            Task<MiddlewareResponse> messageTask = pipeline.Invoke(incomingMessage);
            messageTask.ContinueWith(task =>
            {
                // message completed. Send messages etc here
                Console.WriteLine("[[[Message ended]]]");

                if (task.Result != null)
                {
                    foreach (var responseMessage in task.Result.Messages)
                    {
                        _client.SendMessage(received =>
                        {
                            if (received.ok)
                            {
                                Console.WriteLine(@"Message: '{0}' received", responseMessage.Text);
                            }
                            else
                            {
                                Console.WriteLine(@"FAILED TO DELIVER MESSAGE '{0}'", responseMessage.Text);
                            }
                        }, responseMessage.Channel, responseMessage.Text);
                    }   
                }
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