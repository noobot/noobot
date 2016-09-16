using System;
using System.Collections.Generic;
using System.Linq;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Toolbox.Plugins;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class ScheduleMiddleware : MiddlewareBase
    {
        private readonly SchedulePlugin _schedulePlugin;

        public ScheduleMiddleware(IMiddleware next, SchedulePlugin schedulePlugin) : base(next)
        {
            _schedulePlugin = schedulePlugin;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule hourly"},
                    Description = "Schedule a command to execute every hour on the current channel. Usage: _schedule hourly @{bot} tell me a joke_",
                    EvaluatorFunc = HourlyHandler,
                },
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule daily"},
                    Description = "Schedule a command to execute every day on the current channel. Usage: _schedule daily @{bot} tell me a joke_",
                    EvaluatorFunc = DayHandler,
                },
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule nightly"},
                    Description = "Schedule a command to execute every day on the current channel. Usage: _schedule nightly @{bot} tell me a joke_",
                    EvaluatorFunc = NightlyHandler,
                },
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule list"},
                    Description = "List all schedules on the current channel",
                    EvaluatorFunc = ListHandlerForChannel,
                },
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule delete"},
                    Description = "Delete a schedule in this channel. You must enter a valid {id}",
                    EvaluatorFunc = DeleteHandlerForChannel,
                },
            };
        }

        private IEnumerable<ResponseMessage> HourlyHandler(IncomingMessage message, string matchedHandle)
        {
            yield return CreateSchedule(message, matchedHandle, TimeSpan.FromHours(1), false);
        }

        private IEnumerable<ResponseMessage> DayHandler(IncomingMessage message, string matchedHandle)
        {
            yield return CreateSchedule(message, matchedHandle, TimeSpan.FromDays(1), false);
        }

        private IEnumerable<ResponseMessage> NightlyHandler(IncomingMessage message, string matchedHandle)
        {
            yield return CreateSchedule(message, matchedHandle, TimeSpan.FromDays(1), true);
        }

        private IEnumerable<ResponseMessage> ListHandlerForChannel(IncomingMessage message, string matchedHandle)
        {
            SchedulePlugin.ScheduleEntry[] schedules = _schedulePlugin.ListSchedulesForChannel(message.Channel);

            if (schedules.Any())
            {
                yield return message.ReplyToChannel("Schedules for channel:");

                string[] scheduleStrings = schedules.Select((x, i) => x.ToString(i)).ToArray();
                yield return message.ReplyToChannel(">>>" + string.Join("\n", scheduleStrings));
            }
            else
            {
                yield return message.ReplyToChannel("No schedules set for this channel.");
            }
        }

        private IEnumerable<ResponseMessage> DeleteHandlerForChannel(IncomingMessage message, string matchedHandle)
        {
            string idString = message.TargetedText.Substring(matchedHandle.Length).Trim();

            int? id = ConvertToInt(idString);

            if (id.HasValue)
            {
                SchedulePlugin.ScheduleEntry[] schedules = _schedulePlugin.ListSchedulesForChannel(message.Channel);

                if (id < 0 || id > (schedules.Length - 1))
                {
                    yield return message.ReplyToChannel($"Woops, unable to delete schedule with id of `{id.Value}`");
                }
                else
                {
                    _schedulePlugin.DeleteSchedule(schedules[id.Value]);
                    yield return message.ReplyToChannel($"Removed schedule: {schedules[id.Value]}");
                }
            }
            else
            {
                yield return message.ReplyToChannel($"Invalid id entered. Try using `schedule list`. ({idString})");
            }
        }

        private static int? ConvertToInt(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private ResponseMessage CreateSchedule(IncomingMessage message, string matchedHandle, TimeSpan timeSpan, bool runOnlyAtNight)
        {
            var schedule = new SchedulePlugin.ScheduleEntry
            {
                Channel = message.Channel,
                ChannelType = message.ChannelType,
                Command = message.TargetedText.Substring(matchedHandle.Length).Trim(),
                RunEvery = timeSpan,
                UserId = message.UserId,
                UserName = message.Username,
                LastRun = DateTime.Now,
                RunOnlyAtNight = runOnlyAtNight
            };

            if (string.IsNullOrEmpty(schedule.Command))
            {
                return message.ReplyToChannel("Please enter a command to be scheduled.");
            }
            else
            {
                _schedulePlugin.AddSchedule(schedule);
                return message.ReplyToChannel($"Schedule created for command '{schedule.Command}'.");
            }
        }
    }
}
