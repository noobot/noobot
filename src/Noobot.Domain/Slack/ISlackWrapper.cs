using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Response;
using SlackConnector.Models;

namespace Noobot.Domain.Slack
{
    public interface ISlackWrapper
    {
        Task Connect();
        void Disconnect();
        Task MessageReceived(SlackMessage message);
        Task SendMessage(ResponseMessage responseMessage);
        string GetUserIdForUsername(string username);
        string GetChannelId(string channelName);
    }
}