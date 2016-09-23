using System;
using System.Linq;
using System.Threading;
using Common.Logging;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.Plugins.StandardPlugins
{
    internal class ConnectionPlugin : IPlugin
    {
        private const string EXPECTED_ERROR_MESSAGE = "Error occured while posting message 'cannot_dm_bot'";
        private readonly NoobotCore _noobotCore;
        private readonly ILog _log;
        private Timer _timer;

        public ConnectionPlugin(NoobotCore noobotCore, ILog log)
        {
            _noobotCore = noobotCore;
            _log = log;
        }

        public void Start()
        {
            _log.Info("Starting connection health plugin...");
            _timer = new Timer(CheckConnectionHealth, null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            _log.Info("Stopping connection health plugin...");
            _timer?.Dispose();
        }

        private void CheckConnectionHealth(object dummyData)
        {
            bool shouldReconnect = false;

            try
            {
                string botUserId = _noobotCore.GetUserIdForUsername(_noobotCore.GetBotUserName());
                var message = new ResponseMessage
                {
                    ResponseType = ResponseType.DirectMessage,
                    UserId = botUserId,
                    Text = "Test Message"
                };
                
                //TODO: Find a better way of testing if the connection is alive...
                _noobotCore.SendMessage(message).Wait(TimeSpan.FromSeconds(30));
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Any() && ex.InnerExceptions[0].Message == EXPECTED_ERROR_MESSAGE)
            {
                _log.Info("Health check passed as expected.");
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                shouldReconnect = true;
            }

            if (shouldReconnect)
            {
                _log.Warn("Looks like we 'might' be disconnected (error detected)");
                EnsureClientIsDisconnected();
                Reconnect();
            }
        }

        private void EnsureClientIsDisconnected()
        {
            try
            {
                _log.Info("Ensuring bot is disconnected...");
                _noobotCore.Disconnect();
            }
            catch (Exception ex)
            {
                _log.Error($"Error while disconnectings: {ex}");
            }
        }

        private void Reconnect()
        {
            try
            {
                _noobotCore.Reconnect();
            }
            catch (Exception ex)
            {
                _log.Error($"Error while reconnecting: {ex}");
            }
        }
    }
}