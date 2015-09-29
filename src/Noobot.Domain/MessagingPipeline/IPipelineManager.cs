using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline
{
    public interface IPipelineManager
    {
        void Initialise();
        IMiddleware GetPipeline();
    }
}