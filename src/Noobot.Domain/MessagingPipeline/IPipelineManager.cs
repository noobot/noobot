using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Middleware;

namespace Noobot.Domain.MessagingPipeline
{
    public interface IPipelineManager
    {
        void Initialise();
        IMiddleware GetPipeline();
    }
}