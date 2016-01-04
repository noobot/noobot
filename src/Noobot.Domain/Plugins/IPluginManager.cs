using System;
using StructureMap.Configuration.DSL;

namespace Noobot.Core.Plugins
{
    public interface IPluginManager
    {
        Registry Initialise(Registry registry);
        Type[] ListPluginTypes();
    }
}