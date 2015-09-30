using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class TestMiddleware : MiddlewareBase
    {
        public TestMiddleware(IMiddleware next) : base(next)
        { }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            if (message.Text.Equals("hi", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("I FOUND HI. Ending call stack now");

                yield return message.ReplyToChannel(string.Format("Hey @{0}, how you doing?", message.Username));
                Thread.Sleep(TimeSpan.FromSeconds(5));
                yield return message.ReplyDirectlyToUser("I know where you live...");
            }
            else
            {
                Console.WriteLine("I shouldn't do anything, but want to test the ordering of the pipeline");
                foreach (ResponseMessage responseMessage in Next(message))
                {
                    yield return responseMessage;
                }
            }
        }
    }
}