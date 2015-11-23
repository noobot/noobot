using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlatFile.Delimited.Attributes;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Plugins;
using Noobot.Domain.Plugins.StandardPlugins;
using Noobot.Domain.Slack;

namespace Noobot.Custom.Plugins
{
    public class PingPlugin : IPlugin
    {
        private readonly object _lock = new object();
        private bool _isRunning;
        private readonly ISlackWrapper _slackWrapper;
        private readonly StoragePlugin _storagePlugin;
        private readonly HashSet<string> _userIds = new HashSet<string>();
        private const string _pingFilename = "pingy";

        public PingPlugin(ISlackWrapper slackWrapper, StoragePlugin storagePlugin)
        {
            _slackWrapper = slackWrapper;
            _storagePlugin = storagePlugin;
        }

        public void Start()
        {
            _isRunning = true;

            lock (_lock)
            {
                var users = _storagePlugin.ReadFile<PingModel>(_pingFilename);
                foreach (PingModel user in users)
                {
                    _userIds.Add(user.UserId);
                }
            }

            Task.Factory.StartNew(() =>
            {
                while (_isRunning)
                {
                    var messagesToSend = new List<ResponseMessage>();

                    lock (_lock)
                    {
                        foreach (string userId in _userIds)
                        {
                            string message = "Ping " + DateTime.Now.ToLongTimeString();
                            messagesToSend.Add(ResponseMessage.DirectUserMessage(userId, message));
                        }
                    }

                    foreach (var message in messagesToSend)
                    {
                        _slackWrapper.SendMessage(message);
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;

            lock (_lock)
            {
                var users = _userIds.Select(x => new PingModel { UserId = x }).ToArray();
                _storagePlugin.SaveFile(_pingFilename, users);
            }
        }

        public void StartPingingUser(string userId)
        {
            lock (_lock)
            {
                if (!_userIds.Contains(userId))
                {
                    _userIds.Add(userId);
                }
            }
        }

        public bool StopPingingUser(string userId)
        {
            lock (_lock)
            {
                if (_userIds.Contains(userId))
                {
                    _userIds.Remove(userId);
                    return true;
                }
            }

            return false;
        }

        public string[] ListPingedUsers()
        {
            lock (_lock)
            {
                return _userIds.ToArray();
            }
        }

        [DelimitedFile(Delimiter = ";", Quotes = "\"")]
        private class PingModel
        {
            [DelimitedField(1)]
            public string UserId { get; set; }
        }
    }
}