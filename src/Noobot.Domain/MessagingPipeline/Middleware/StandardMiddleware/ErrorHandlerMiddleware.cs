using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    /// <summary>
    /// Handles unhanlded exceptions. Should just log it somewhere
    /// </summary>
    public class ErrorHandlerMiddleware : MiddlewareBase
    {
        public ErrorHandlerMiddleware(IMiddleware next) : base(next)
        { }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            try
            {
                return Next(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine("ERROR DETECTED: {0}", exception);
            }

            return new ResponseMessage[0];
        }
    }
}