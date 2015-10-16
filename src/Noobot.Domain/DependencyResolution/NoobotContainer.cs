using System;
using System.Collections.Generic;
using Noobot.Domain.Plugins;
using Noobot.Domain.Slack;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Noobot.Domain.DependencyResolution
{
    public class NoobotContainer : Container, INoobotContainer
    {
        private readonly Type[] _pluginTypes;

        public NoobotContainer(Registry registry, Type[] pluginTypes) : base(registry)
        {
            _pluginTypes = pluginTypes;
        }

        public ISlackWrapper GetSlackConnector()
        {
            return GetInstance<ISlackWrapper>();
        }

        public IPlugin[] GetPlugins()
        {
            var result = new List<IPlugin>(_pluginTypes.Length);

            foreach (Type pluginType in _pluginTypes)
            {
                IPlugin plugin = GetInstance(pluginType) as IPlugin;
                if (plugin == null)
                {
                    string error = string.Format("Plugin failed to build {0}", pluginType);
                    throw new NullReferenceException(error);
                }

                result.Add(plugin);
            }

            return result.ToArray();
        }
    }
}