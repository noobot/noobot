using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace Noobot.Core.DependencyResolution
{
    public class ContainerFactory : IContainerFactory
    {
        private readonly IPipelineConfiguration _pipelineConfiguration;
        private readonly IPluginConfiguration _pluginConfiguration;

        private readonly Type[] _singletons =
        {
            typeof(INoobotCore),
            typeof(IPipelineFactory),
            typeof(IConfigReader),
        };

        public ContainerFactory(IPipelineConfiguration pipelineConfiguration, IPluginConfiguration pluginConfiguration)
        {
            _pipelineConfiguration = pipelineConfiguration;
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

            registry.For<IPipelineFactory>().Use<PipelineFactory>();
            registry.For<INoobotCore>().Use<NoobotCore>();

            SetupSingletons(registry);
            SetupMiddlewarePipeline(registry);
            Type[] pluginTypes = SetupPlugins(registry);

            var container = new NoobotContainer(registry, pluginTypes);

            IPipelineFactory pipelineFactory = container.GetInstance<IPipelineFactory>();
            pipelineFactory.SetContainer(container);

            return container;
        }

        private void SetupSingletons(Registry registry)
        {
            foreach (Type type in _singletons)
            {
                registry.For(type).Singleton();
            }
        }

        private void SetupMiddlewarePipeline(Registry registry)
        {
            Stack<Type> pipeline = GetPipelineStack();

            registry.Scan(x =>
            {
                // scan assemblies that we are loading pipelines from
                foreach (Type middlewareType in pipeline)
                {
                    x.AssemblyContainingType(middlewareType);
                }
            });

            registry.For<IMiddleware>().Use<UnhandledMessageMiddleware>();

            while (pipeline.Any())
            {
                Type nextType = pipeline.Pop();
                var nextDeclare = registry.For<IMiddleware>();

                MethodInfo decorateMethod = nextDeclare.GetType().GetMethod("DecorateAllWith", new[] { typeof(Func<Instance, bool>) });
                MethodInfo generic = decorateMethod.MakeGenericMethod(nextType);
                generic.Invoke(nextDeclare, new object[] { null });
            }

            registry.For<IMiddleware>().DecorateAllWith<AboutMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<ScheduleMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<StatsMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<AdminMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<HelpMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<BeginMessageMiddleware>();
        }

        private Stack<Type> GetPipelineStack()
        {
            List<Type> pipelineList = _pipelineConfiguration.ListMiddlewareTypes();

            var pipeline = new Stack<Type>();
            foreach (Type type in pipelineList)
            {
                pipeline.Push(type);
            }

            return pipeline;
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