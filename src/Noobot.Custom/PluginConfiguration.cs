using System;
using Noobot.Core.Plugins;
using Noobot.Custom.Plugins;

namespace Noobot.Custom
{
    public class PluginConfiguration : IPluginConfiguration
    {
        public Type[] ListPluginTypes()
        {
            return new[]
            {
                typeof(StoragePlugin),
                typeof(SchedulePlugin),
                typeof(AdminPlugin),
                typeof(PingPlugin)
            };
        }
    }
}