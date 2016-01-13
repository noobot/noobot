using System;
using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.Plugins;
using StructureMap;

namespace Noobot.Core.DependencyResolution
{
    internal class NoobotContainer : INoobotContainer
    {
        private Container _container;
        private readonly Type[] _pluginTypes;

        public NoobotContainer(Type[] pluginTypes)
        {
            _pluginTypes = pluginTypes;
        }

        public void Initialise(Registry registry)
        {
            _container = new Container(registry);
        }

        public INoobotCore GetNoobotCore()
        {
            return _container.GetInstance<INoobotCore>();
        }

        private IPlugin[] _plugins;
        public IPlugin[] GetPlugins()
        {
            if (_plugins == null)
            {
                var result = new List<IPlugin>(_pluginTypes.Length);

                foreach (Type pluginType in _pluginTypes)
                {
                    IPlugin plugin = _container.GetInstance(pluginType) as IPlugin;
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

        public T GetPlugin<T>() where T : class, IPlugin
        {
            return _container.TryGetInstance(typeof(T)) as T;
        }

        public IMiddleware GetMiddlewarePipeline()
        {
            return _container.GetInstance<IMiddleware>();
        }
    }
}