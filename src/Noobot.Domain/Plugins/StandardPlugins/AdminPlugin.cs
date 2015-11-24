using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Noobot.Domain.Configuration;

namespace Noobot.Domain.Plugins.StandardPlugins
{
    public class AdminPlugin : IPlugin
    {
        private readonly IConfigReader _configReader;
        private readonly HashSet<string> _admins = new HashSet<string>();
        private readonly object _lock = new object();
        private int? _adminPin = null;

        public AdminPlugin(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public void Start()
        {
            JObject config = _configReader.GetConfig();
            _adminPin = config.Value<int?>("adminPin");

            if (_adminPin.HasValue)
            {
                Console.WriteLine($"Admin pin is '{_adminPin.Value}'");
            }
            else
            {
                Console.WriteLine("No admin pin detected. Admin mode deactivated.");
            }
        }

        public void Stop()
        { }

        public bool AdminModeEnabled()
        {
            return _adminPin.HasValue;
        }

        public bool AuthoriseUser(string user, int pin)
        {
            bool authorised = false;

            if (_adminPin.HasValue)
            {
                authorised = pin == _adminPin.Value;

                if (authorised)
                {
                    lock (_lock)
                    {
                        _admins.Add(user);
                    }
                }
            }

            return authorised;
        }

        public bool AuthenticateUser(string user)
        {
            lock (_lock)
            {
                return _admins.Contains(user);
            }
        }
    }
}