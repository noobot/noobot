using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Noobot.Domain.Configuration;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class AdminMiddleware : MiddlewareBase
    {
        private readonly IConfigReader _configReader;
        private static int? _adminPin = null;

        public AdminMiddleware(IMiddleware next, IConfigReader configReader) : base(next)
        {
            _configReader = configReader;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{ "admin" },
                    EvaluatorFunc = AdminHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> AdminHandler(IncomingMessage message, string matchedHandle)
        {
            int adminPin = GetAdminPin();

            yield return message.ReplyDirectlyToUser("Admin area");
        }

        private int GetAdminPin()
        {
            JObject config = _configReader.GetConfig();
            if (!_adminPin.HasValue)
            {
                _adminPin = config.Value<int>("adminPin");
            }

            return _adminPin.Value;
        }
    }
}