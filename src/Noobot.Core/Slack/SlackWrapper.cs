using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Request.Extensions;
using Noobot.Core.MessagingPipeline.Response;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Core.Slack
{
    public class SlackWrapper : ISlackWrapper
    {
        private readonly IConfigReader _configReader;
        private readonly IPipelineFactory _pipelineFactory;
        private ISlackConnection _connection;

        public SlackWrapper(IConfigReader configReader, IPipelineFactory pipelineFactory)
        {
            _configReader = configReader;
            _pipelineFactory = pipelineFactory;
        }

        public async Task Connect()
        {
            JObject config = _configReader.GetConfig();
            string slackKey = config["slack"].Value<string>("apiToken");

            var connector = new SlackConnector.SlackConnector();
            _connection = await connector.Connect(slackKey);
            _connection.OnMessageReceived += MessageReceived;
            _connection.OnDisconnect += OnDisconnect;

            Console.WriteLine("Connected!");
            Console.WriteLine($"Bots Name: {_connection.Self.Name}");
            Console.WriteLine($"Team Name: {_connection.Team.Name}");
        }

        private bool _isDisconnecting;
        public void Disconnect()
        {
            _isDisconnecting = true;

            if (_connection != null && _connection.IsConnected)
            {
                _connection.Disconnect();
            }
        }

        private void OnDisconnect()
        {
            if (_isDisconnecting)
            {
                Console.WriteLine("Disconnected.");
            }
            else
            {
                Console.WriteLine("Disconnected from server, attempting to reconnect...");
                _connection.OnMessageReceived -= MessageReceived;
                _connection.OnDisconnect -= OnDisconnect;
                _connection = null;

                Connect()
                    .ContinueWith(task =>
                    {
                        if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                        {
                            Console.WriteLine("Connection restored.");
                        }
                        else
                        {
                            Console.WriteLine($"Error while reconnecting: {task.Exception}");
                        }
                    });
            }
        }

        public async Task MessageReceived(SlackMessage message)
        {
            Console.WriteLine("[[[Message started]]]");

            IMiddleware pipeline = _pipelineFactory.GetPipeline();
            var incomingMessage = new IncomingMessage
            {
                RawText = message.Text,
                FullText = message.Text,
                UserId = message.User.Id,
                Username = GetUsername(message),
                Channel = message.ChatHub.Id,
                ChannelType = message.ChatHub.Type == SlackChatHubType.DM ? ResponseType.DirectMessage : ResponseType.Channel,
                UserChannel = await GetUserChannel(message),
                BotName = _connection.Self.Name,
                BotId = _connection.Self.Id,
                BotIsMentioned = message.MentionsBot
            };

            incomingMessage.TargetedText = incomingMessage.GetTargetedText();

            try
            {
                foreach (ResponseMessage responseMessage in pipeline.Invoke(incomingMessage))
                {
                    await SendMessage(responseMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR WHILE PROCESSING MESSAGE: {0}", ex);
            }

            Console.WriteLine("[[[Message ended]]]");
        }

        public async Task SendMessage(ResponseMessage responseMessage)
        {
            SlackChatHub chatHub = await GetChatHub(responseMessage);

            if (chatHub != null)
            {
                if (responseMessage is TypingIndicatorMessage)
                {
                    await _connection.IndicateTyping(chatHub);
                }
                else
                {
                    var botMessage = new BotMessage
                    {
                        ChatHub = chatHub,
                        Text = responseMessage.Text,
                        Attachments = GetAttachments(responseMessage.Attachment)
                    };

                    await _connection.Say(botMessage);
                }
            }
            else
            {
                Console.WriteLine("Unable to find channel for message '{0}'. Message not sent", responseMessage.Text);
            }
        }

        private IList<SlackAttachment> GetAttachments(Attachment attachment)
        {
            var attachments = new List<SlackAttachment>();

            if (attachment != null)
            {
                attachments.Add(new SlackAttachment
                {
                    Fallback = attachment.Fallback,
                    ImageUrl = attachment.ImageUrl,
                    ThumbUrl = attachment.ThumbUrl,
                    AuthorName = attachment.AuthorName
                });
            }

            return attachments;
        }

        public string GetUserIdForUsername(string username)
        {
            var user = _connection.UserNameCache.FirstOrDefault(x => x.Value.Equals(username, StringComparison.InvariantCultureIgnoreCase));
            return string.IsNullOrEmpty(user.Key) ? string.Empty : user.Key;
        }

        public string GetChannelId(string channelName)
        {
            var channel = _connection.ConnectedChannels().FirstOrDefault(x => x.Name.Equals(channelName, StringComparison.InvariantCultureIgnoreCase));
            return channel != null ? channel.Id : string.Empty;
        }

        public Dictionary<string, string> ListChannels()
        {
            return _connection.ConnectedHubs.Values.ToDictionary(channel => channel.Id, channel => channel.Name);
        }

        private string GetUsername(SlackMessage message)
        {
            return _connection.UserNameCache.ContainsKey(message.User.Id) ? _connection.UserNameCache[message.User.Id] : string.Empty;
        }

        private async Task<string> GetUserChannel(SlackMessage message)
        {
            return (await GetUserChatHub(message.User.Id, joinChannel: false) ?? new SlackChatHub()).Id;
        }

        private async Task<SlackChatHub> GetChatHub(ResponseMessage responseMessage)
        {
            SlackChatHub chatHub = null;

            if (responseMessage.ResponseType == ResponseType.Channel)
            {
                chatHub = new SlackChatHub { Id = responseMessage.Channel };
            }
            else if (responseMessage.ResponseType == ResponseType.DirectMessage)
            {
                if (string.IsNullOrEmpty(responseMessage.Channel))
                {
                    chatHub = await GetUserChatHub(responseMessage.UserId);
                }
                else
                {
                    chatHub = new SlackChatHub { Id = responseMessage.Channel };
                }
            }

            return chatHub;
        }

        private async Task<SlackChatHub> GetUserChatHub(string userId, bool joinChannel = true)
        {
            SlackChatHub chatHub = null;

            if (_connection.UserNameCache.ContainsKey(userId))
            {
                string username = "@" + _connection.UserNameCache[userId];
                chatHub = _connection.ConnectedDMs().FirstOrDefault(x => x.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase));
            }

            if (chatHub == null && joinChannel)
            {
                chatHub = await _connection.JoinDirectMessageChannel(userId);
            }

            return chatHub;
        }
    }
}