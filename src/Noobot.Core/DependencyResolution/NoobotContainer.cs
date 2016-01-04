using System;
using System.Collections.Generic;
using Noobot.Core.Plugins;
using Noobot.Core.Slack;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Noobot.Core.DependencyResolution
{
    public class NoobotContainer : Container, INoobotContainer
    {
        private readonly Type[] _pluginTypes;

        public NoobotContainer(Registry registry, Type[] pluginTypes) : base(registry)
        {
            _pluginTypes = pluginTypes;
        }

        public INoobotCore GetNoobotCore()
        {
            return GetInstance<INoobotCore>();
        }

        public IPlugin[] GetPlugins()
        {
            var result = new List<IPlugin>(_pluginTypes.Length);

            foreach (Type pluginType in _pluginTypes)
            {
                IPlugin plugin = GetInstance(pluginType) as IPlugin;
                if (plugin == null)
                {
                    throw new NullReferenceException($"Plugin failed to build {pluginType}");
                }

                result.Add(plugin);
            }

            return result.ToArray();
        }
    }
}