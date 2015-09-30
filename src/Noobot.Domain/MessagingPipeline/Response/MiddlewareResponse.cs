using System.Collections.Generic;

namespace Noobot.Domain.MessagingPipeline.Response
{
    public class MiddlewareResponse
    {
        public List<ResponseMessage> Messages { get; set; }

        public MiddlewareResponse()
        {
            Messages = new List<ResponseMessage>();
        }
    }
}