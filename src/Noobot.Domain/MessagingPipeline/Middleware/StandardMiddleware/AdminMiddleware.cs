using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Noobot.Domain.Configuration;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using Noobot.Domain.Plugins.StandardPlugins;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class AdminMiddleware : MiddlewareBase
    {
        private readonly AdminPlugin _adminPlugin;

        public AdminMiddleware(IMiddleware next, AdminPlugin adminPlugin) : base(next)
        {
            _adminPlugin = adminPlugin;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin pin" },
                    EvaluatorFunc = AdminPinHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> AdminPinHandler(IncomingMessage message, string matchedHandle)
        {
            if (!_adminPlugin.AdminModeEnabled())
            {
                yield return message.ReplyToChannel("Admin mode isn't enabled.");
                yield break;
            }
            
            string pinString = message.TargetedText.Substring(matchedHandle.Length).Trim();

            int pin = 0;
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
        
    }
}