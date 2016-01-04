using System.Collections.Generic;
using System.Threading.Tasks;
using Noobot.Core.MessagingPipeline.Response;
using SlackConnector.Models;

namespace Noobot.Core.Slack
{
    public interface ISlackWrapper
    {
        Task Connect();
        void Disconnect();
        Task MessageReceived(SlackMessage message);
        Task SendMessage(ResponseMessage responseMessage);
        string GetUserIdForUsername(string username);
        string GetChannelId(string channelName);
        Dictionary<string, string> ListChannels();
    }
}