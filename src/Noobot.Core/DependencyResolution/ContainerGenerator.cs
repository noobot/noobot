using System;
using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline;
using Noobot.Core.Plugins;
using Noobot.Core.Slack;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Noobot.Core.DependencyResolution
{
    public class ContainerGenerator : IContainerGenerator
    {
        private readonly IPipelineManager _pipelineManager;
        private readonly IPluginManager _pluginManager;

        private readonly Type[] _singletons = 
        {
            typeof(ISlackWrapper),
            typeof(IPipelineFactory),
            typeof(IConfigReader),
        };

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

            foreach (Type type in _singletons)
            {
                registry.For(type).Singleton();
            }

            Type[] pluginTypes = _pluginManager.ListPluginTypes();
            var container = new NoobotContainer(registry, pluginTypes);

            IPipelineFactory pipelineFactory = container.GetInstance<IPipelineFactory>();
            pipelineFactory.SetContainer(container);

            return container;
        }
    }
}