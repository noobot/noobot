using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline
{
    public interface IPipelineManager
    {
        MiddlewareBase GetPipeline();
    }
}