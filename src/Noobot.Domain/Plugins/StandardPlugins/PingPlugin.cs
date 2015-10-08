using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Slack;

namespace Noobot.Domain.Plugins.StandardPlugins
{
    public class PingPlugin : IPlugin
    {
        private readonly object _lock = new object();
        private bool _isRunning;
        private readonly ISlackConnector _slackConnector;
        private readonly StringCollection _userIds = new StringCollection();

        public PingPlugin(ISlackConnector slackConnector)
        {
            _slackConnector = slackConnector;
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
                            messagesToSend.Add(new ResponseMessage
                            {
                                UserId = userId,
                                Text = "Ping " + DateTime.Now.ToLongTimeString(),
                                ResponseType = ResponseType.DirectMessage
                            });
                        }
                    }

                    foreach (var message in messagesToSend)
                    {
                        _slackConnector.SendMessage(message);
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void PingUserId(string userId)
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
    }
}