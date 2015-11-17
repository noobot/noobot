using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using FlatFile.Delimited.Attributes;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Plugins;
using Noobot.Domain.Slack;
using Noobot.Domain.Storage;
using SlackConnector.Models;

namespace Noobot.Custom.Plugins
{
    public class SchedulePlugin : IPlugin
    {
        private string FileName { get; } = "schedules";
        private readonly IStorageHelper _storageHelper;
        private readonly ISlackWrapper _slackWrapper;
        private readonly object _lock = new object();
        private readonly List<ScheduleEntry> _schedules = new List<ScheduleEntry>();
        private readonly Timer _timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);

        public SchedulePlugin(IStorageHelper storageHelper, ISlackWrapper slackWrapper)
        {
            _storageHelper = storageHelper;
            _slackWrapper = slackWrapper;
        }

        public void Start()
        {
            lock (_lock)
            {
                ScheduleEntry[] schedules = _storageHelper.ReadFile<ScheduleEntry>(FileName);
                _schedules.AddRange(schedules);

                _timer.Elapsed += RunSchedules;
                _timer.Start();
            }
        }

        public void Stop()
        {
            _timer.Stop();
            Save();
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
                ScheduleEntry[] schedules = _schedules.Where(x => x.Channel == channel).ToArray();
                return schedules;
            }
        }

        private void RunSchedules(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                foreach (var schedule in _schedules)
                {
                    if (ShouldRunSchedule(schedule))
                    {
                        SlackChatHubType channelType = schedule.ChannelType == ResponseType.Channel
                            ? SlackChatHubType.Channel
                            : SlackChatHubType.DM;

                        var slackMessage = new SlackMessage
                        {
                            Text = schedule.Command,
                            User = new SlackUser { Id = schedule.UserId, Name = schedule.UserName },
                            ChatHub = new SlackChatHub {Id = schedule.Channel,Type = channelType},
                        };

                        _slackWrapper.MessageReceived(slackMessage).Wait();
                        schedule.LastRun = DateTime.Now;
                    }
                }
            }

            Save();
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

            if (shouldRun & schedule.RunOnlyAtNight)
            {
                shouldRun = DateTime.Now.TimeOfDay > new TimeSpan(0) && DateTime.Now.TimeOfDay < TimeSpan.FromHours(4);
            }

            return shouldRun;
        }

        private void Save()
        {
            lock (_lock)
            {
                _storageHelper.SaveFile(FileName, _schedules.ToArray());
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
        }
    }
}
