using System;
using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware
{
    public class Mapping
    {
        public string[] ValidHandle { get; set; }
        public Func<IncomingMessage, IEnumerable<ResponseMessage>> EvaluatorFunc { get; set; }
        public string Description { get; set; } 
    }
}