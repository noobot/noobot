using System;
using System.Threading.Tasks;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class TestMiddleware : MiddlewareBase
    {
        public TestMiddleware(IMiddleware next) : base(next)
        { }

        public override async Task<MiddlewareResponse> Invoke(IncomingMessage message)
        {
            if (message.Text.Equals("hi", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("I FOUND HI. Ending call stack now");
                var response = new MiddlewareResponse();
                response.Messages.Add(new ResponseMessage
                {
                    Channel = message.Channel,
                    Text = string.Format("Hey @{0}, how you doing?", message.Username)
                });

                return response;
            }

            Console.WriteLine("I shouldn't do anything, but want to test the ordering of the pipeline");
            return await Next(message);
        }
    }
}