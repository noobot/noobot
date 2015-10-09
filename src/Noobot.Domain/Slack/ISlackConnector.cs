using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.Slack
{
    public interface ISlackConnector
    {
        Task<InitialConnectionStatus> Connect();
        void Disconnect();
        void SendMessage(ResponseMessage responseMessage);
        string GetUserIdForUsername(string username);
        string GetChannelId(string channelName);
    }
}