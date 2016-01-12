using System;

namespace Noobot.Core.Configuration
{
    public interface IConfiguration
    {
        Type[] ListMiddlewareTypes();
        Type[] ListPluginTypes();
    }
}