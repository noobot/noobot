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
            if (message.Text.Equals("hi", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("I FOUND HI. Ending call stack now");
                return await Task.FromResult<Response>(null);
            }

            Console.WriteLine("I shouldn't do anything, but want to test the ordering of the pipeline");
            return await Next(message);
        }
    }
}