using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using LetsAgree.IOC;
using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Core.DependencyResolution
{
    class ContainerFactory<C,R,T> : IContainerFactory
        where R : IBasicRegistration<C, T>, IGenericRegistration<C, T>, IScanningRegistraction<C, T>
        where C : ISingletonConfig, IDecoratorConfig
        where T : IBasicContainer, IGenericContainer
    {
        private readonly IConfigReader _configReader;
        private readonly IConfiguration _configuration;
        private readonly ILog _logger;
        private readonly IRegistryCreator<C, R, T> _registryCreator;

        public ContainerFactory(IConfiguration configuration, IConfigReader configReader, IRegistryCreator<C, R, T> registryCreator,  ILog logger = null)
        {
            _registryCreator = registryCreator;
            _configuration = configuration;
            _configReader = configReader;
            _logger = logger;
        }

        public INoobotContainer CreateContainer()            
        {
            var registry = CreateRegistry();

            SetupMiddlewarePipeline(registry);
            Type[] pluginTypes = SetupPlugins(registry);

            registry.Register<INoobotCore, NoobotCore>().AsSingleton();
            registry.Register(() => _logger).AsSingleton();
            registry.Register(() => _configReader).AsSingleton();

            INoobotContainer container = CreateContainer(pluginTypes, registry);
            return container;
        }

        private R CreateRegistry()
        {
            var registry = _registryCreator.GenerateRegistry();

            // setups DI for everything in Noobot.Core
            registry.RegisterAssembly(GetType().Assembly);

            return registry;
        }

        private static INoobotContainer CreateContainer(Type[] pluginTypes, R registry)
        {
            var container = new NoobotContainer<T>(pluginTypes);
            registry.Register<INoobotContainer>(() => container).AsSingleton();
            container.Initialise(registry.GenerateContainer());
            return container;
        }

        private void SetupMiddlewarePipeline(R registry)
        {
            var pipeline = GetPipelineStack();
            var pipelineAssemblies = pipeline.Select(x => x.Assembly)
                                             .Distinct();

            foreach (var pa in pipelineAssemblies)
            {
                registry.RegisterAssembly(pa);
            }

            registry.Register<IMiddleware, UnhandledMessageMiddleware>();

            if (_configReader.AboutEnabled)
            {
                registry.Register<IMiddleware, AboutMiddleware>().AsDecorator();
            }

            if (_configReader.StatsEnabled)
            {
                registry.Register<IMiddleware, StatsMiddleware>().AsDecorator();
            }

            while (pipeline.Any())
            {
                registry.Register(typeof(IMiddleware), pipeline.Pop()).AsDecorator();
            }

            if (_configReader.HelpEnabled)
            {
                registry.Register<IMiddleware, HelpMiddleware>().AsDecorator();
            }

            registry.Register<IMiddleware,BeginMessageMiddleware>().AsDecorator();
        }

        private Stack<Type> GetPipelineStack()
        {
            Type[] pipelineList = _configuration.ListMiddlewareTypes() ?? new Type[0];

            var pipeline = new Stack<Type>();
            foreach (Type type in pipelineList)
            {
                pipeline.Push(type);
            }

            return pipeline;
        }

        private Type[] SetupPlugins(R registry)
        {
            var pluginTypes = new List<Type>
            {
                typeof(StatsPlugin)
            };

            Type[] customPlugins = _configuration.ListPluginTypes() ?? new Type[0];
            pluginTypes.AddRange(customPlugins);

            var pluginAssemblies = pluginTypes.Select(x => x.Assembly)
                                              .Distinct();

            foreach (var assembly in pluginAssemblies)
            {
                registry.RegisterAssembly(assembly);
            }

            // make all plugins singletons
            foreach (Type pluginType in pluginTypes)
            {
                registry.Register(pluginType, pluginType)
                        .AsSingleton();
            }

            return pluginTypes.ToArray();
        }
    }
}