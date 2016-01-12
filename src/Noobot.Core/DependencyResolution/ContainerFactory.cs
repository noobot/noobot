using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noobot.Core.Configuration;
using Noobot.Core.Logging;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware;
using Noobot.Core.Plugins.StandardPlugins;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace Noobot.Core.DependencyResolution
{
    public class ContainerFactory : IContainerFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigReader _configReader;
        private readonly ILog _logger;

        private readonly Type[] _singletons =
        {
            typeof(INoobotCore),
            typeof(IConfigReader),
        };

        public ContainerFactory(IConfiguration configuration, IConfigReader configReader)
            : this(configuration, configReader, null)
        { }

        public ContainerFactory(IConfiguration configuration, IConfigReader configReader, ILog logger)
        {
            _configuration = configuration;
            _configReader = configReader;
            _logger = logger ?? new EmptyLog();
        }

        public INoobotContainer CreateContainer()
        {
            var registry = new Registry();
            registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });

            SetupSingletons(registry);
            SetupMiddlewarePipeline(registry);
            Type[] pluginTypes = SetupPlugins(registry);

            registry.For<INoobotCore>().Use<NoobotCore>();
            registry.For<ILog>().Use(() => _logger);
            registry.For<IConfigReader>().Use(() => _configReader);

            var container = CreateContainer(pluginTypes, registry);

            return container;
        }

        private static NoobotContainer CreateContainer(Type[] pluginTypes, Registry registry)
        {
            var container = new NoobotContainer(pluginTypes);
            registry.For<INoobotContainer>().Use(x => container);
            container.Initialise(registry);
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

            // defined here so they can be overridden
            registry.For<IMiddleware>().DecorateAllWith<AboutMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<StatsMiddleware>();

            while (pipeline.Any())
            {
                Type nextType = pipeline.Pop();
                var nextDeclare = registry.For<IMiddleware>();

                MethodInfo decorateMethod = nextDeclare.GetType().GetMethod("DecorateAllWith", new[] { typeof(Func<Instance, bool>) });
                MethodInfo generic = decorateMethod.MakeGenericMethod(nextType);
                generic.Invoke(nextDeclare, new object[] { null });
            }

            registry.For<IMiddleware>().DecorateAllWith<HelpMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<BeginMessageMiddleware>();
        }

        private Stack<Type> GetPipelineStack()
        {
            Type[] pipelineList = _configuration.ListMiddlewareTypes();

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
                typeof (StatsPlugin)
            };

            pluginTypes.AddRange(_configuration.ListPluginTypes());

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