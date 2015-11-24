using System;
using System.Collections.Generic;
using System.Linq;
using Noobot.Domain.Configuration;

namespace Noobot.Domain.Plugins.StandardPlugins
{
    public class AdminPlugin : IPlugin
    {
        private readonly IConfigReader _configReader;
        private readonly HashSet<string> _admins = new HashSet<string>();
        private readonly object _lock = new object();

        public AdminPlugin(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public void Start()
        { }

        public void Stop()
        { }
    }
}