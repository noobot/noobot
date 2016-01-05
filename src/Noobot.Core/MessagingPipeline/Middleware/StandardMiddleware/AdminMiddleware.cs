using System.Collections.Generic;
using System.Linq;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class AdminMiddleware : MiddlewareBase
    {
        private readonly AdminPlugin _adminPlugin;
        private readonly SchedulePlugin _schedulePlugin;
        private readonly INoobotCore _noobotCore;

        public AdminMiddleware(IMiddleware next, AdminPlugin adminPlugin, SchedulePlugin schedulePlugin, INoobotCore noobotCore) : base(next)
        {
            _adminPlugin = adminPlugin;
            _schedulePlugin = schedulePlugin;
            _noobotCore = noobotCore;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin pin" },
                    EvaluatorFunc = PinHandler
                },
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin schedules list" },
                    EvaluatorFunc = SchedulesListHandler
                },
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin schedules delete" },
                    EvaluatorFunc = DeleteSchedulesHandler
                },
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin channels" },
                    EvaluatorFunc = ChannelsHandler
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