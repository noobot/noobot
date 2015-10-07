using Noobot.Domain.DependencyResolution;
using Noobot.Domain.MessagingPipeline.Middleware;

namespace Noobot.Domain.MessagingPipeline
{
    public interface IPipelineFactory
    {
        void SetContainer(INoobotContainer noobotContainer);
        IMiddleware GetPipeline();
    }
}