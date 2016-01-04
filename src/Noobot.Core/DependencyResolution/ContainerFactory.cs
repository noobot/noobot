using System;
using System.Collections.Generic;
using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using Noobot.Core.Slack;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Noobot.Core.DependencyResolution
{
    public class ContainerFactory : IContainerFactory
    {
        private readonly IPipelineManager _pipelineManager;
        private readonly IPluginConfiguration _pluginConfiguration;

        private readonly Type[] _singletons =
        {
            typeof(INoobotCore),
            typeof(IPipelineFactory),
            typeof(IConfigReader),
        };

        public ContainerFactory(IPipelineManager pipelineManager, IPluginConfiguration pluginConfiguration)
        {
            _pipelineManager = pipelineManager;
            _pluginConfiguration = pluginConfiguration;
        }

        public INoobotContainer CreateContainer()
        {
            var registry = new Registry();

            registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });

            foreach (Type type in _singletons)
            {
                registry.For(type).Singleton();
            }

            registry = _pipelineManager.Initialise(registry);
            Type[] pluginTypes = SetupPlugins(registry);

            var container = new NoobotContainer(registry, pluginTypes);

            IPipelineFactory pipelineFactory = container.GetInstance<IPipelineFactory>();
            pipelineFactory.SetContainer(container);

            return container;
        }

        private Type[] SetupPlugins(Registry registry)
        {
            var pluginTypes = new List<Type>
            {
                typeof (StoragePlugin),
                typeof (SchedulePlugin),
                typeof (StatsPlugin),
                typeof (AdminPlugin)
            };

            pluginTypes.AddRange(_pluginConfiguration.ListPluginTypes());

            registry.Scan(x =>
            {
                // scan assemblies that we are loading pipelines from
                foreach (Type pluginType in pluginTypes)
                {
                    x.AssemblyContainingType(pluginType);
                }
            });

            // make all plugins singletons
            foreach (Type pluginType in pluginTypes)
            {
                registry
                    .For(pluginType)
                    .Singleton();
            }

            return pluginTypes.ToArray();
        }
    }
}