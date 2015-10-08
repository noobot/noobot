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
                    foreach (string userId in _userIds)
                    {
                        var message = new ResponseMessage
                        {
                            UserId = userId,
                            Text = "Ping " + DateTime.Now.ToLongTimeString(),
                            ResponseType = ResponseType.DirectMessage
                        };

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
            if (!_userIds.Contains(userId))
            {
                _userIds.Add(userId);
            }
        }

        public bool StopPingingUser(string userId)
        {
            if (_userIds.Contains(userId))
            {
                _userIds.Remove(userId);
                return true;
            }

            return false;
        }
    }
}