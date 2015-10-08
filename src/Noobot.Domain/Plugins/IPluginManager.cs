using System;
using StructureMap.Configuration.DSL;

namespace Noobot.Domain.Plugins
{
    public interface IPluginManager
    {
        Registry Initialise(Registry registry);
        Type[] ListPluginTypes();
    }
}