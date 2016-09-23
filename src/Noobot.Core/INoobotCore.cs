using System.Collections.Generic;
using System.Threading.Tasks;
using Noobot.Core.MessagingPipeline.Response;
using SlackConnector.Models;

namespace Noobot.Core
{
    public interface INoobotCore
    {
        Task Connect();
        void Disconnect();
        Task MessageReceived(SlackMessage message);
        Task SendMessage(ResponseMessage responseMessage);
        string GetUserIdForUsername(string username);
        string GetChannelId(string channelName);
        string GetBotUserName();
        Dictionary<string, string> ListChannels();
    }
}