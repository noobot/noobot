using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.Slack
{
    public interface ISlackWrapper
    {
        Task Connect();
        void Disconnect();
        Task SendMessage(ResponseMessage responseMessage);
        string GetUserIdForUsername(string username);
        string GetChannelId(string channelName);
    }
}