using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noobot.Custom.Plugins;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Custom.Pipeline.Middleware
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
                    Description = "Schedule a command to execute every hour on the current channel",
                    EvaluatorFunc = HourlyHandler,
                },
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule daily"},
                    Description = "Schedule a command to execute every day on the current channel",
                    EvaluatorFunc = DayHandler,
                },
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule nightly"},
                    Description = "Schedule a command to execute every day on the current channel",
                    EvaluatorFunc = NightlyHandler,
                },
                new HandlerMapping
                {
                    ValidHandles = new [] { "schedule list"},
                    Description = "List all schedules on the current channel",
                    EvaluatorFunc = ListHandlerForChannel,
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
                yield return message.ReplyToChannel(">>>" + string.Join("\n", FormatSchedulesForText(schedules)));
            }
            else
            {
                yield return message.ReplyToChannel("No schedules set for this channel.");
            }
        }

        private static string[] FormatSchedulesForText(SchedulePlugin.ScheduleEntry[] schedules)
        {
            return schedules
                  .Select(x => $"Running command '{x.Command}' every '{x.RunEvery}'. Last run at '{x.LastRun}'. Runs only at night: {x.RunOnlyAtNight.ToString()}.")
                  .ToArray();
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