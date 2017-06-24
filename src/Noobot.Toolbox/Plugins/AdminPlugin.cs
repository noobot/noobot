using System.Collections.Generic;
using Common.Logging;
using Noobot.Core.Configuration;
using Noobot.Core.Plugins;

namespace Noobot.Toolbox.Plugins
{
    internal class AdminPlugin : IPlugin
    {
        private const string ADMINPIN_CONFIGENTRY = "adminPin";
        private readonly IConfigReader _configReader;
        private readonly ILog _log;
        private readonly HashSet<string> _admins = new HashSet<string>();
        private readonly object _lock = new object();
        private int? _adminPin;

        public AdminPlugin(IConfigReader configReader, ILog log)
        {
            _configReader = configReader;
            _log = log;
        }

        public void Start()
        {
            _adminPin = _configReader.GetConfigEntry<int?>(ADMINPIN_CONFIGENTRY);

            if (_adminPin.HasValue)
            {
                _log.Info($"Admin pin is '{_adminPin.Value}'");
            }
            else
            {
                _log.Info("No admin pin detected. Admin mode deactivated.");
            }
        }

        public void Stop() { }

        public bool AdminModeEnabled() => _adminPin.HasValue;

        public bool AuthoriseUser(string userId, int pin)
        {
            if(pin != _adminPin)
                return false; // Keep the users away from the key section, exit early.

            lock (_lock)
            {
                _admins.Add(userId);
                return true;
            }
        }

        public bool AuthenticateUser(string userId)
        {
            lock (_lock)
            {
                return _admins.Contains(userId);
            }
        }
    }
}