using System;
using System.Linq;
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
        private readonly IPipelineFactory _pipelineFactory;
        private SlackSocketClient _client;
        private string _myId;
        private string _myName;

        public SlackConnector(IConfigReader configReader, IPipelineFactory pipelineFactory)
        {
            _configReader = configReader;
            _pipelineFactory = pipelineFactory;
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
                _myName = loginResponse.self.name;
            },
            () =>
            {
                //This is called once the Real Time Messaging client has connected to the end point
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

            string text = FilterText(newMessage.text);
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            Console.WriteLine("[[[Message started]]]");

            IMiddleware pipeline = _pipelineFactory.GetPipeline();
            var incomingMessage = new IncomingMessage
            {
                MessageId = newMessage.id,
                Text = text,
                UserId = newMessage.user,
                Channel = newMessage.channel,
                Username = _client.UserLookup[newMessage.user].name,
                UserChannel = GetUserChannel(newMessage)
            };

            Task.Factory.StartNew(() =>
            {
                foreach (ResponseMessage responseMessage in pipeline.Invoke(incomingMessage))
                {
                    SendMessage(responseMessage);
                }
            })
            .ContinueWith(task => Console.WriteLine("[[[Message ended]]]"));
        }

        public void SendMessage(ResponseMessage responseMessage)
        {
            string channel = responseMessage.Channel;
            if (responseMessage.ResponseType == ResponseType.DirectMessage)
            {
                if (string.IsNullOrEmpty(channel) || !_client.DirectMessageLookup.ContainsKey(channel))
                {
                    channel = JoinDirectMessageChannel(responseMessage.UserId);
                }
            }
            
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
            }, channel, responseMessage.Text);
        }

        private string JoinDirectMessageChannel(string userName)
        {
            var tcs = new TaskCompletionSource<string>();

            _client.JoinDirectMessageChannel(response =>
            {
                if (response.ok)
                {
                    tcs.SetResult(response.channel.id);
                }
                else
                {
                    tcs.SetResult(string.Empty);
                }
            }, userName);

            return tcs.Task.Result;
        }

        private string FilterText(string text)
        {
            string[] myNames =
            {
                _myName + ":",
                _myName,
                string.Format("<@{0}>:", _myId),
                string.Format("<@{0}>", _myId),
                string.Format("@{0}:", _myName),
                string.Format("@{0}", _myName),
            };

            string match = myNames.FirstOrDefault(x => text.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
            return string.IsNullOrEmpty(match) ? string.Empty : text.Substring(match.Length).Trim();
        }

        private string GetUserChannel(NewMessage newMessage)
        {
            var channel = _client.DirectMessages.FirstOrDefault(x => x.user == newMessage.user);
            if (channel != null)
            {
                return channel.id;
            }

            return newMessage.user;
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