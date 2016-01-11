using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Noobot.Core.Logging;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Request.Extensions;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Core
{
    internal class NoobotCore : INoobotCore
    {
        private readonly IConfigReader _configReader;
        private readonly ILog _log;
        private readonly INoobotContainer _container;
        private ISlackConnection _connection;

        public NoobotCore(IConfigReader configReader, ILog  log ,INoobotContainer container)
        {
            _configReader = configReader;
            _log = log;
            _container = container;
        }

        public async Task Connect()
        {
            string slackKey = _configReader.SlackApiKey();

            var connector = new SlackConnector.SlackConnector();
            _connection = await connector.Connect(slackKey);
            _connection.OnMessageReceived += MessageReceived;
            _connection.OnDisconnect += OnDisconnect;
            
            _log.Log("Connected!");
            _log.Log($"Bots Name: {_connection.Self.Name}");
            _log.Log($"Team Name: {_connection.Team.Name}");

            StartPlugins();
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
                _log.Log("Disconnected.");
                StopPlugins();
            }
            else
            {
                _log.Log("Disconnected from server, attempting to reconnect...");
                _connection.OnMessageReceived -= MessageReceived;
                _connection.OnDisconnect -= OnDisconnect;
                _connection = null;

                Connect()
                    .ContinueWith(task =>
                    {
                        if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                        {
                            _log.Log("Connection restored.");
                            _container.GetPlugin<StatsPlugin>().IncrementState("ConnectionsRestored");
                        }
                        else
                        {
                            _log.Log($"Error while reconnecting: {task.Exception}");
                        }
                    });
            }
        }

        public async Task MessageReceived(SlackMessage message)
        {
            _log.Log("[[[Message started]]]");

            IMiddleware pipeline = _container.GetMiddlewarePipeline();
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
                _log.Log($"ERROR WHILE PROCESSING MESSAGE: {ex}");
            }

            _log.Log("[[[Message ended]]]");
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
                _log.Log($"Unable to find channel for message '{responseMessage.Text}'. Message not sent");
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

        private void StartPlugins()
        {
            IPlugin[] plugins = _container.GetPlugins();
            foreach (IPlugin plugin in plugins)
            {
                plugin.Start();
            }
        }

        private void StopPlugins()
        {
            IPlugin[] plugins = _container.GetPlugins();
            foreach (IPlugin plugin in plugins)
            {
                plugin.Stop();
            }
        }
    }
}