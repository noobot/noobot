using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Plugins;
using Noobot.Domain.Slack;

namespace Noobot.Custom.Plugins
{
    public class PingPlugin : IPlugin
    { 
        private readonly object _lock = new object();
        private bool _isRunning;
        private readonly ISlackWrapper _slackWrapper;
        private readonly HashSet<string> _userIds = new HashSet<string>();

        public PingPlugin(ISlackWrapper slackWrapper)
        {
            _slackWrapper = slackWrapper;
        }

        public void Start()
        {
            _isRunning = true;

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
    }
}