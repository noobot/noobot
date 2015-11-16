using System;
using System.Collections.Generic;
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
                _schedulePlugin.Addchedule(schedule);
                return message.ReplyToChannel($"Schedule created for command '{schedule.Command}'.");
            }
        }
    }
}