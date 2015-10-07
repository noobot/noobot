using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.Slack;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Noobot.Domain.DependencyResolution
{
    public class ContainerGenerator : IContainerGenerator
    {
        private readonly IPipelineManager _pipelineManager;

        public ContainerGenerator(IPipelineManager pipelineManager)
        {
            _pipelineManager = pipelineManager;
        }

        public INoobotContainer Generate()
        {
            var registry = new Registry();

            registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });

            registry = _pipelineManager.Initialise(registry);

            registry
                .For<ISlackConnector>()
                .Singleton();

            registry
                .For<IPipelineFactory>()
                .Singleton();

            var container = new NoobotContainer(registry);

            var pipelineFactory = container.GetInstance<IPipelineFactory>();
            pipelineFactory.SetContainer(container);

            return container;
        }
    }
}