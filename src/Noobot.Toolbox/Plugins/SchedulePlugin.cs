using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Common.Logging;
using FlatFile.Delimited.Attributes;
using Noobot.Core;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using SlackConnector.Models;

namespace Noobot.Toolbox.Plugins
{
    internal class SchedulePlugin : IPlugin
    {
        private string FileName { get; } = "schedules";

        private readonly StoragePlugin _storagePlugin;
        private readonly INoobotCore _noobotCore;
        private readonly StatsPlugin _statsPlugin;
        private readonly ILog _log;
        private readonly object _lock = new object();
        private readonly List<ScheduleEntry> _schedules = new List<ScheduleEntry>();
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);

        private static bool IsCurrentlyNight() => DateTime.Now.TimeOfDay > TimeSpan.FromHours(20) || DateTime.Now.TimeOfDay < TimeSpan.FromHours(5);

        public SchedulePlugin(StoragePlugin storagePlugin, INoobotCore noobotCore, StatsPlugin statsPlugin, ILog log)
        {
            _storagePlugin = storagePlugin;
            _noobotCore = noobotCore;
            _statsPlugin = statsPlugin;
            _log = log;
        }

        public void Start()
        {
            lock (_lock)
            {
                _schedules.AddRange(_storagePlugin.ReadFile<ScheduleEntry>(FileName));

                _timer.Elapsed += RunSchedules;
                _timer.Start();
            }
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void AddSchedule(ScheduleEntry schedule)
        {
            lock (_lock)
            {
                _schedules.Add(schedule);
            }

            Save();
        }

        public ScheduleEntry[] ListSchedulesForChannel(string channel)
        {
            lock (_lock)
            {
                return _schedules
                    .Where(x => x.Channel == channel)
                    .OrderBy(x => x.RunEvery)
                    .ThenByDescending(x => x.LastRun)
                    .ThenBy(x => x.Command)
                    .ToArray();
            }
        }

        public ScheduleEntry[] ListAllSchedules()
        {
            lock (_lock)
            {
                return _schedules
                    .OrderBy(x => x.RunEvery)
                    .ThenByDescending(x => x.LastRun)
                    .ThenBy(x => x.Command)
                    .ToArray();
            }
        }

        public void DeleteSchedules(ScheduleEntry[] scheduleEntries)
        {
            lock (_lock)
            {
                foreach (ScheduleEntry scheduleEntry in scheduleEntries)
                {
                    _schedules.Remove(scheduleEntry);
                }
            }

            Save();
        }

        public void DeleteSchedule(ScheduleEntry scheduleEntry)
        {
            DeleteSchedules(new[] { scheduleEntry });
        }

        private void RunSchedules(object sender, ElapsedEventArgs e)
       {
            lock (_lock)
            {
                _statsPlugin.RecordStat("Schedules:LastRun", DateTime.Now.ToString("G"));
                _statsPlugin.RecordStat("Schedules:IsCurrentlyNight", IsCurrentlyNight().ToString());

                foreach (var schedule in _schedules)
                {
                    ExecuteSchedule(schedule);
                }
            }
        }

        private void ExecuteSchedule(ScheduleEntry schedule)
        {
            if (ShouldRunSchedule(schedule))
            {
                _log.Info($"Running schedule: {schedule}");

                SlackChatHubType channelType = schedule.ChannelType == ResponseType.Channel
                    ? SlackChatHubType.Channel
                    : SlackChatHubType.DM;

                var slackMessage = new SlackMessage
                {
                    Text = schedule.Command,
                    User = new SlackUser { Id = schedule.UserId, Name = schedule.UserName },
                    ChatHub = new SlackChatHub { Id = schedule.Channel, Type = channelType },
                };

                _noobotCore.MessageReceived(slackMessage);
                schedule.LastRun = DateTime.Now;
            }
        }

        private static bool ShouldRunSchedule(ScheduleEntry schedule)
        {
            bool shouldRun = false;
            if (!schedule.LastRun.HasValue)
            {
                shouldRun = true;
            }
            else if (schedule.LastRun + schedule.RunEvery < DateTime.Now)
            {
                shouldRun = true;
            }

            if (shouldRun && schedule.RunOnlyAtNight)
            {
                shouldRun = IsCurrentlyNight();
            }

            return shouldRun;
        }

        private void Save()
        {
            lock (_lock)
            {
                _storagePlugin.SaveFile(FileName, _schedules.ToArray());
                _statsPlugin.RecordStat("Schedules:Active", _schedules.Count);
            }
        }

        [DelimitedFile(Delimiter = ";", Quotes = "\"")]
        public class ScheduleEntry
        {
            [DelimitedField(1, NullValue = "=Null")]
            public DateTime? LastRun { get; set; }
            [DelimitedField(2)]
            public TimeSpan RunEvery { get; set; }
            [DelimitedField(3)]
            public string Command { get; set; }
            [DelimitedField(4)]
            public string Channel { get; set; }
            [DelimitedField(5)]
            public ResponseType ChannelType { get; set; }
            [DelimitedField(6)]
            public string UserId { get; set; }
            [DelimitedField(7)]
            public string UserName { get; set; }
            [DelimitedField(8)]
            public bool RunOnlyAtNight { get; set; }

            public override string ToString() => $"Running command `'{Command}'` every `'{RunEvery}'`. Last run at `'{LastRun}'`. Runs only at night: `{RunOnlyAtNight}`.";

            public string ToString(int id) => $"Id: `{id}`. {this}";
        }
    }
}
