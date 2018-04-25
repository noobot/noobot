using System;
using System.Collections.Generic;
using LetsAgree.IOC;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.Plugins;

namespace Noobot.Core.DependencyResolution
{
    internal class NoobotContainer<T> : INoobotContainer
        where T : IBasicContainer, IGenericContainer
    {
        private T _container;
        private readonly Type[] _pluginTypes;

        public NoobotContainer(Type[] pluginTypes)
        {
            _pluginTypes = pluginTypes;
        }

        public void Initialise(T container)
        {
            _container = container;
        }

        public INoobotCore GetNoobotCore()
        {
            return _container.Resolve<INoobotCore>();
        }

        private IPlugin[] _plugins;
        public IPlugin[] GetPlugins()
        {
            if (_plugins == null)
            {
                var result = new List<IPlugin>(_pluginTypes.Length);

                foreach (Type pluginType in _pluginTypes)
                {
                    IPlugin plugin = _container.Resolve(pluginType) as IPlugin;
                    if (plugin == null)
                    {
                        throw new NullReferenceException($"Plugin failed to build {pluginType}");
                    }

                    result.Add(plugin);
                }

                _plugins = result.ToArray();
            }

            return _plugins;
        }

        public P GetPlugin<P>() where P : class, IPlugin
        {
            return _container.TryResolve(out P r) ? r : null;
        }

        public IMiddleware GetMiddlewarePipeline()
        {
            return _container.Resolve<IMiddleware>();
        }

    }
}