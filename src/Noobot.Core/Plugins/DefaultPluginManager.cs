using System;

namespace Noobot.Core.Plugins
{
    /// <summary>
    /// Default implementation of IPluginConfiguration
    /// </summary>
    public sealed class DefaultPluginManager : IPluginConfiguration
    {
        public Type[] ListPluginTypes()
        {
            return new Type[0];
        }
    }
}