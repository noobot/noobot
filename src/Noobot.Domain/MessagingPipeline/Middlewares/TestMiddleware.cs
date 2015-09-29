using System;
using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline.Middlewares
{
    public class TestMiddleware : MiddlewareBase
    {
        public TestMiddleware(IMiddleware next) : base(next)
        { }

        public override async Task<Response> Invoke(IncomingMessage message)
        {
            Console.WriteLine("I shouldn't do anything, but want to test the ordering of the pipeline");
            return await Next(message);
        }
    }
}