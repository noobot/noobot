using System;
using System.Linq;
using System.Threading.Tasks;
using Noobot.Domain.Configuration;
using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Domain.Slack
{
    public class SlackWrapper : ISlackWrapper
    {
        private readonly IConfigReader _configReader;
        private readonly IPipelineFactory _pipelineFactory;
        private ISlackConnector _client;

        public SlackWrapper(IConfigReader configReader, IPipelineFactory pipelineFactory)
        {
            _configReader = configReader;
            _pipelineFactory = pipelineFactory;
        }

        public async Task Connect()
        {
            var config = _configReader.GetConfig();

            _client = new SlackConnector.SlackConnector();
            _client.OnMessageReceived += MessageReceived;
            _client.OnConnectionStatusChanged += ConnectionStatusChanged;

            await _client.Connect(config.Slack.ApiToken);
        }

        private void ConnectionStatusChanged(bool isConnected)
        {
            Console.WriteLine(isConnected ? "CONNECTED :) x999" : "Bot is no longer connected :(");
        }

        private async Task MessageReceived(ResponseContext messageContext)
        {
            Console.WriteLine("[[[Message started]]]");
            SlackMessage message = messageContext.Message;

            IMiddleware pipeline = _pipelineFactory.GetPipeline();
            var incomingMessage = new IncomingMessage
            {
                Text = message.Text,
                UserId = message.User.Id,
                Username = _client.UserNameCache[message.User.Id],
                Channel = message.ChatHub.Id,
                //TODO
                // UserChannel = _client.ConnectedDMs.FirstOrDefault(x => x.Name.Equals(_client.UserNameCache[message.User.Id])).Id,
                BotName = _client.UserName,
                BotId = _client.UserId,
                BotIsMentioned = message.MentionsBot
            };

            try
            {
                foreach (ResponseMessage responseMessage in pipeline.Invoke(incomingMessage))
                {
                    await SendMessage(responseMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex);
            }

            Console.WriteLine("[[[Message ended]]]");
        }

        public async Task SendMessage(ResponseMessage responseMessage)
        {
            SlackChatHub chatHub = null;

            if (responseMessage.ResponseType == ResponseType.Channel)
            {
                chatHub = SlackChatHub.FromID(responseMessage.Channel);
            }
            else if (responseMessage.ResponseType == ResponseType.DirectMessage)
            {
                if (_client.UserNameCache.ContainsKey(responseMessage.UserId))
                {
                    string username = "@" + _client.UserNameCache[responseMessage.UserId];
                    chatHub = _client.ConnectedDMs.FirstOrDefault(x => x.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            if (chatHub != null)
            {
                var botMessage = new BotMessage
                {
                    ChatHub = _client.ConnectedHubs[responseMessage.Channel],
                    Text = responseMessage.Text
                };

                await _client.Say(botMessage);
            }
            else
            {
                Console.WriteLine("Unable to find channel for message '{0}'. Message not sent", responseMessage.Text);
            }


            //string channel = responseMessage.Channel;
            //if (responseMessage.ResponseType == ResponseType.DirectMessage)
            //{
            //    if (string.IsNullOrEmpty(channel) || !_client.DirectMessageLookup.ContainsKey(channel))
            //    {
            //        channel = JoinDirectMessageChannel(responseMessage.UserId);
            //    }
            //}

            //_client.SendMessage(received =>
            //{
            //    if (received.ok)
            //    {
            //        Console.WriteLine(@"Message: '{0}' received", responseMessage.Text);
            //    }
            //    else
            //    {
            //        Console.WriteLine(@"FAILED TO DELIVER MESSAGE '{0}'", responseMessage.Text);
            //    }
            //}, channel, responseMessage.Text);
        }

        public string GetUserIdForUsername(string username)
        {
            var user = _client.UserNameCache.FirstOrDefault(x => x.Value.Equals(username, StringComparison.InvariantCultureIgnoreCase));
            return string.IsNullOrEmpty(user.Key) ? string.Empty : user.Key;
        }

        public string GetChannelId(string channelName)
        {
            var channel = _client.ConnectedChannels.FirstOrDefault(x => x.Name.Equals(channelName, StringComparison.InvariantCultureIgnoreCase));
            return channel != null ? channel.Id : string.Empty;
        }

        //private string JoinDirectMessageChannel(string userName)
        //{
        //    var tcs = new TaskCompletionSource<string>();

        //    _client.JoinDirectMessageChannel(response =>
        //    {
        //        if (response.ok)
        //        {
        //            tcs.SetResult(response.channel.id);
        //        }
        //        else
        //        {
        //            tcs.SetResult(string.Empty);
        //        }
        //    }, userName);

        //    return tcs.Task.Result;
        //}

        //private string GetUserChannel(NewMessage newMessage)
        //{
        //    var channel = _client.DirectMessages.FirstOrDefault(x => x.user == newMessage.user);
        //    if (channel != null)
        //    {
        //        return channel.id;
        //    }

        //    return newMessage.user;
        //}

        public void Disconnect()
        {
            if (_client != null && _client.IsConnected)
            {
                _client.Disconnect();
            }
        }
    }
}