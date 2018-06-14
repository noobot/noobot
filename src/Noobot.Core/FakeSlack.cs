using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SlackConnector;
using SlackConnector.EventHandlers;
using SlackConnector.Models;

namespace Noobot.Core
{
    class FakeContactDetails : ContactDetails
    {
        public FakeContactDetails(string id, String name)
        {
            GetType().GetProperty("Id").SetValue(this, id);
            GetType().GetProperty("Name").SetValue(this, name);
        }
    }
    class FakeSlack : ISlackConnector, ISlackConnection
    {
        private FakeSlack() { }
        public static readonly ISlackConnector Instance = new FakeSlack();

        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs => new Dictionary<String, SlackChatHub>();

        public IReadOnlyDictionary<string, SlackUser> UserCache => new Dictionary<String,SlackUser>();

        public bool IsConnected => true;

        public DateTime? ConnectedSince => DateTime.Now;

        public string SlackKey => "1234";

        public ContactDetails Team => new FakeContactDetails("1", "MockTeam");

        public ContactDetails Self => new FakeContactDetails("0","Mock");

        public event DisconnectEventHandler OnDisconnect;
        public event ReconnectEventHandler OnReconnecting;
        public event ReconnectEventHandler OnReconnect;
        public event MessageReceivedEventHandler OnMessageReceived;
        public event ReactionReceivedEventHandler OnReaction;
        public event ChatHubJoinedEventHandler OnChatHubJoined;
        public event UserJoinedEventHandler OnUserJoined;
        public event PongEventHandler OnPong;

        public Task ArchiveChannel(string channelName)
        {
            return Task.CompletedTask;
        }

        public Task Close()
        {
            return Task.CompletedTask;
        }

        public async Task<ISlackConnection> Connect(string slackKey)
        {
            await Task.Delay(200);
            return this;
        }

        public Task<SlackChatHub> CreateChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            
        }

        public Task<IEnumerable<SlackChatHub>> GetChannels()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SlackUser>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task IndicateTyping(SlackChatHub chatHub)
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> JoinChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> JoinDirectMessageChannel(string user)
        {
            throw new NotImplementedException();
        }

        public Task Ping()
        {
            throw new NotImplementedException();
        }

        public Task Say(BotMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<SlackPurpose> SetChannelPurpose(string channelName, string purpose)
        {
            throw new NotImplementedException();
        }

        public Task<SlackTopic> SetChannelTopic(string channelName, string topic)
        {
            throw new NotImplementedException();
        }

        public Task Upload(SlackChatHub chatHub, string filePath)
        {
            throw new NotImplementedException();
        }

        public Task Upload(SlackChatHub chatHub, Stream stream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}