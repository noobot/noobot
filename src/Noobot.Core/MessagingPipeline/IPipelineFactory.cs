using Noobot.Core.DependencyResolution;
using Noobot.Core.MessagingPipeline.Middleware;

namespace Noobot.Core.MessagingPipeline
{
    public interface IPipelineFactory
    {
        void SetContainer(INoobotContainer noobotContainer);
        IMiddleware GetPipeline();
    }
}