using System;
using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline.StandardMiddleware
{
    /// <summary>
    /// Handles unhanlded exceptions. Should just log it somewhere
    /// </summary>
    public class ErrorHandlerMiddleware : MiddlewareBase
    {
        public ErrorHandlerMiddleware(IMiddleware next) : base(next)
        { }

        public override async Task<Response> Invoke(IncomingMessage message)
        {
            try
            {
                return await Next(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine("ERROR DETECTED: {0}", exception);
            }

            return await Task.FromResult<Response>(null);
        }
    }
}