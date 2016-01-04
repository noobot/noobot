using StructureMap.Configuration.DSL;

namespace Noobot.Core.MessagingPipeline
{
    public interface IPipelineManager
    {
        Registry Initialise(Registry registry);
    }
}