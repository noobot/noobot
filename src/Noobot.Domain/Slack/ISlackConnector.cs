using System.Threading.Tasks;

namespace Noobot.Domain.Slack
{
    public interface ISlackConnector
    {
        Task<InitialConnectionStatus> Connect();
        void Disconnect();
    }
}