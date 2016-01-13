using System.Collections.Generic;
using System.Linq;
using Noobot.Core;
using Noobot.Core.Logging;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Toolbox.Plugins;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    internal class AdminMiddleware : MiddlewareBase
    {
        private readonly AdminPlugin _adminPlugin;
        private readonly SchedulePlugin _schedulePlugin;
        private readonly INoobotCore _noobotCore;
        private readonly ILog _log;

        public AdminMiddleware(IMiddleware next, AdminPlugin adminPlugin, SchedulePlugin schedulePlugin, INoobotCore noobotCore, ILog log) : base(next)
        {
            _adminPlugin = adminPlugin;
            _schedulePlugin = schedulePlugin;
            _noobotCore = noobotCore;
            _log = log;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin pin" },
                    EvaluatorFunc = PinHandler,
                    Description = "This function is used to authenticate a user as admin",
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin schedules list" },
                    EvaluatorFunc = SchedulesListHandler,
                    Description = "[Requires authentication] Will return a list of all schedules.",
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin schedules delete" },
                    EvaluatorFunc = DeleteSchedulesHandler,
                    Description = "[Requires authentication] This will delete all schedules.",
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin channels" },
                    EvaluatorFunc = ChannelsHandler,
                    Description = "[Requires authentication] Will return all channels connected.",
                    VisibleInHelp = false
                }
            };
        }

        private IEnumerable<ResponseMessage> PinHandler(IncomingMessage message, string matchedHandle)
        {
            if (!_adminPlugin.AdminModeEnabled())
            {
                yield return message.ReplyToChannel("Admin mode isn't enabled.");
                yield break;
            }

            string pinString = message.TargetedText.Substring(matchedHandle.Length).Trim();

            int pin;
            if (int.TryParse(pinString, out pin))
            {
                if (_adminPlugin.AuthoriseUser(message.UserId, pin))
                {
                    yield return message.ReplyToChannel($"{message.Username} - you now have admin rights.");
                    _log.Log($"{message.Username} now has admin rights.");
                }
                else
                {
                    yield return message.ReplyToChannel("Incorrect admin pin entered.");
                }
            }
            else
            {
                yield return message.ReplyToChannel($"Unable to parse pin '{pinString}'");
            }
        }

        private IEnumerable<ResponseMessage> SchedulesListHandler(IncomingMessage message, string matchedHandle)
        {
            if (!_adminPlugin.AuthenticateUser(message.UserId))
            {
                yield return message.ReplyToChannel($"Sorry {message.Username}, only admins can use this function.");
                yield break;
            }

            var schedules = _schedulePlugin.ListAllSchedules();
            string[] scheduleStrings = schedules.Select((x, i) => x.ToString(i) + $" Channel: '{x.Channel}'.").ToArray();

            yield return message.ReplyToChannel("All Schedules:");
            yield return message.ReplyToChannel(">>>" + string.Join("\n", scheduleStrings));
        }

        private IEnumerable<ResponseMessage> DeleteSchedulesHandler(IncomingMessage message, string matchedHandle)
        {
            if (!_adminPlugin.AuthenticateUser(message.UserId))
            {
                yield return message.ReplyToChannel($"Sorry {message.Username}, only admins can use this function.");
                yield break;
            }

            var schedules = _schedulePlugin.ListAllSchedules();
            _schedulePlugin.DeleteSchedules(schedules);

            yield return message.ReplyToChannel("All schedules deleted");
        }

        private IEnumerable<ResponseMessage> ChannelsHandler(IncomingMessage message, string matchedHandle)
        {
            if (!_adminPlugin.AuthenticateUser(message.UserId))
            {
                yield return message.ReplyToChannel($"Sorry {message.Username}, only admins can use this function.");
                yield break;
            }

            Dictionary<string, string> channels = _noobotCore.ListChannels();
            yield return message.ReplyToChannel("All Connected Channels:");
            yield return message.ReplyToChannel(">>>" + string.Join("\n", channels.Select(x => $"{x.Key}: {x.Value}")));
        }
    }
}