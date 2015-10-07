using Noobot.Domain.MessagingPipeline.Middleware;
using StructureMap.Configuration.DSL;

namespace Noobot.Domain.MessagingPipeline
{
    public interface IPipelineManager
    {
        Registry Initialise(Registry registry);
        IMiddleware GetPipeline();
    }
}