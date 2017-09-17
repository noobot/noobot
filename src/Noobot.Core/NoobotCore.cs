using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
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
        private readonly AverageStat _averageResponse;
        private ISlackConnection _connection;

        public NoobotCore(IConfigReader configReader, ILog log, INoobotContainer container)
        {
            _configReader = configReader;
            _log = log;
            _container = container;
            _averageResponse = new AverageStat("milliseconds");
        }

        public async Task Connect()
        {
            string slackKey = _configReader.SlackApiKey;

            var connector = new SlackConnector.SlackConnector();
            _connection = await connector.Connect(slackKey);
            _connection.OnMessageReceived += MessageReceived;
            _connection.OnDisconnect += OnDisconnect;
            _connection.OnReconnecting += OnReconnecting;
            _connection.OnReconnect += OnReconnect;

            _log.Info("Connected!");
            _log.Info($"Bots Name: {_connection.Self.Name}");
            _log.Info($"Team Name: {_connection.Team.Name}");

            _container.GetPlugin<StatsPlugin>()?.RecordStat("Connected:Since", DateTime.Now.ToString("G"));
            _container.GetPlugin<StatsPlugin>()?.RecordStat("Response:Average", _averageResponse);

            StartPlugins();
        }

        private Task OnReconnect()
        {
            _log.Info("Connection Restored!");
            _container.GetPlugin<StatsPlugin>().IncrementState("ConnectionsRestored");
            return Task.CompletedTask;
        }

        private Task OnReconnecting()
        {
            _log.Info("Attempting to reconnect to Slack...");
            _container.GetPlugin<StatsPlugin>().IncrementState("Reconnecting");
            return Task.CompletedTask;
        }

        private bool _isDisconnecting;
        public void Disconnect()
        {
            _isDisconnecting = true;

            if (_connection != null && _connection.IsConnected)
            {
                _connection
                    .Close()
                    .GetAwaiter()
                    .GetResult();
            }
        }

        private void OnDisconnect()
        {
            StopPlugins();

            if (_isDisconnecting)
            {
                _log.Info("Disconnected.");
            }
            else
            {
                _log.Info("Disconnected from server, attempting to reconnect...");
                Reconnect();
            }
        }

        internal void Reconnect()
        {
            _log.Info("Reconnecting...");
            if (_connection != null)
            {
                _connection.OnMessageReceived -= MessageReceived;
                _connection.OnDisconnect -= OnDisconnect;
                _connection = null;
            }

            _isDisconnecting = false;
            Connect()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        _log.Info("Connection restored.");
                        _container.GetPlugin<StatsPlugin>().IncrementState("ConnectionsRestored");
                    }
                    else
                    {
                        _log.Info($"Error while reconnecting: {task.Exception}");
                    }
                });
        }

        public async Task MessageReceived(SlackMessage message)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _log.Info($"[Message found from '{message.User.Name}']");

            IMiddleware pipeline = _container.GetMiddlewarePipeline();
            var incomingMessage = new IncomingMessage
            {
                RawText = message.Text,
                FullText = message.Text,
                UserId = message.User.Id,
                Username = GetUsername(message),
                UserEmail = message.User.Email,
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
                _log.Error($"ERROR WHILE PROCESSING MESSAGE: {ex}");
            }

            stopwatch.Stop();

            _log.Info($"[Message ended - Took {stopwatch.ElapsedMilliseconds} milliseconds]");
            _averageResponse.Log(stopwatch.ElapsedMilliseconds);
        }

        public async Task Ping()
        {
            await _connection.Ping();
        }

        public async Task SendMessage(ResponseMessage responseMessage)
        {
            SlackChatHub chatHub = await GetChatHub(responseMessage);

            if (chatHub != null)
            {
                if (responseMessage is TypingIndicatorMessage)
                {
                    _log.Info($"Indicating typing on channel '{chatHub.Name}'");
                    await _connection.IndicateTyping(chatHub);
                }
                else
                {
                    var botMessage = new BotMessage
                    {
                        ChatHub = chatHub,
                        Text = responseMessage.Text,
                        Attachments = responseMessage.Attachments
                    };

                    string textTrimmed = botMessage.Text.Length > 50 ? botMessage.Text.Substring(0, 50) + "..." : botMessage.Text;
                    _log.Info($"Sending message '{textTrimmed}'");
                    await _connection.Say(botMessage);
                }
            }
            else
            {
                _log.Error($"Unable to find channel for message '{responseMessage.Text}'. Message not sent");
            }
        }

        public string GetUserIdForUsername(string username)
        {
            var user = _connection.UserCache.FirstOrDefault(x => x.Value.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(user.Key) ? string.Empty : user.Key;
        }

        public string GetChannelId(string channelName)
        {
            var channel = _connection.ConnectedChannels().FirstOrDefault(x => x.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
            return channel != null ? channel.Id : string.Empty;
        }

        public Dictionary<string, string> ListChannels()
        {
            return _connection.ConnectedHubs.Values.ToDictionary(channel => channel.Id, channel => channel.Name);
        }

        public string GetBotUserName()
        {
            return _connection?.Self.Name;
        }

        private string GetUsername(SlackMessage message)
        {
            return _connection.UserCache.ContainsKey(message.User.Id) ? _connection.UserCache[message.User.Id].Name : string.Empty;
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

            if (_connection.UserCache.ContainsKey(userId))
            {
                string username = "@" + _connection.UserCache[userId].Name;
                chatHub = _connection.ConnectedDMs().FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            if (chatHub == null && joinChannel)
            {
                chatHub = await _connection.JoinDirectMessageChannel(userId);
            }

            return chatHub;
        }

        /// <summary>
        /// TODO: Move these methods into container?
        /// </summary>
        private void StartPlugins()
        {
            IPlugin[] plugins = _container.GetPlugins();
            foreach (IPlugin plugin in plugins)
            {
                plugin.Start();
            }
        }

        /// <summary>
        /// TODO: Move these methods into container?
        /// </summary>
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