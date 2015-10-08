using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Configuration.DSL;

namespace Noobot.Domain.Plugins
{
    public abstract class PluginManagerBase : IPluginManager
    {
        private readonly List<Type> _pluginTypes = new List<Type>(); 

        protected abstract void Initialise();
        public Registry Initialise(Registry registry)
        {
            Initialise();

            registry.Scan(x =>
            {
                // scan assemblies that we are loading pipelines from
                MethodInfo method = x.GetType().GetMethod("AssemblyContainingType", new Type[0]);
                foreach (Type pluginType in _pluginTypes)
                {
                    MethodInfo generic = method.MakeGenericMethod(pluginType);
                    generic.Invoke(x, null);
                }
            });

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