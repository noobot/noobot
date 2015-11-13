using System;
using Noobot.Domain.Configuration;
using Noobot.Domain.MessagingPipeline;
using Noobot.Domain.Plugins;
using Noobot.Domain.Slack;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Noobot.Domain.DependencyResolution
{
    public class ContainerGenerator : IContainerGenerator
    {
        private readonly IPipelineManager _pipelineManager;
        private readonly IPluginManager _pluginManager;

        public ContainerGenerator(IPipelineManager pipelineManager, IPluginManager pluginManager)
        {
            _pipelineManager = pipelineManager;
            _pluginManager = pluginManager;
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
            registry = _pluginManager.Initialise(registry);

            registry
                .For<ISlackWrapper>()
                .Singleton();

            registry
                .For<IPipelineFactory>()
                .Singleton();

            registry
                .For<IConfigReader>()
                .Singleton();

            Type[] pluginTypes = _pluginManager.ListPluginTypes();
            var container = new NoobotContainer(registry, pluginTypes);

            IPipelineFactory pipelineFactory = container.GetInstance<IPipelineFactory>();
            pipelineFactory.SetContainer(container);

            return container;
        }
    }
}