using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline
{
    public interface IPipelineManager
    {
        Task<Response> Invoke(IncomingMessage message);
    }
}