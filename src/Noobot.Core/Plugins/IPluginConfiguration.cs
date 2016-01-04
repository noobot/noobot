using System;

namespace Noobot.Core.Plugins
{
    public interface IPluginConfiguration
    {
        Type[] ListPluginTypes();
    }
}