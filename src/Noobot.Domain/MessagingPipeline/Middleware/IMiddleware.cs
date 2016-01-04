using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.MessagingPipeline.Middleware
{
    public interface IMiddleware
    {
        IEnumerable<ResponseMessage> Invoke(IncomingMessage message);
        IEnumerable<CommandDescription> GetSupportedCommands();
    }
}