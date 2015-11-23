using System;
using System.Collections.Generic;
using Noobot.Domain.Plugins.StandardPlugins;
using StructureMap.Configuration.DSL;

namespace Noobot.Domain.Plugins
{
    public abstract class PluginManagerBase : IPluginManager
    {
        private readonly List<Type> _pluginTypes = new List<Type>(); 

        protected abstract void Initialise();
        public Registry Initialise(Registry registry)
        {
            _pluginTypes.Add(typeof(StoragePlugin));
            _pluginTypes.Add(typeof(SchedulePlugin));
            _pluginTypes.Add(typeof(StatsPlugin));

            Initialise();

            registry.Scan(x =>
            {
                // scan assemblies that we are loading pipelines from
                foreach (Type pluginType in _pluginTypes)
                {
                    x.AssemblyContainingType(pluginType);
                }
            });

            // make all plugins singletons
            foreach (Type pluginType in _pluginTypes)
            {
                registry
                    .For(pluginType)
                    .Singleton();
            }

            return registry;
        }

        public Type[] ListPluginTypes()
        {
            return _pluginTypes.ToArray();
        }

        protected void Use<T>() where T : IPlugin
        {
            _pluginTypes.Add(typeof(T));
        }
    }
}