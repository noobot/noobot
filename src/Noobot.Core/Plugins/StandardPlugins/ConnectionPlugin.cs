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
                _noobotCore.Ping().Wait(TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                _log.Error($"WebSocket Ping Failed: {ex}");
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