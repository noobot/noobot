using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware
{
    public interface IMiddleware
    {
        IEnumerable<ResponseMessage> Invoke(IncomingMessage message);
    }
}