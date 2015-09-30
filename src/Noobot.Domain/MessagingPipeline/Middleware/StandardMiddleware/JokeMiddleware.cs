using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using RestSharp;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    public class JokeMiddleware : MiddlewareBase
    {
        public JokeMiddleware(IMiddleware next) : base(next)
        { }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            if (message.Text.Equals("joke", StringComparison.InvariantCultureIgnoreCase))
            {
                yield return message.ReplyToChannel("Hmm... let me think");

                var client = new RestClient("http://tambal.azurewebsites.net");
                var request = new RestRequest("/joke/random", Method.GET);
                var result = client.Execute<JokeContainer>(request);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    yield return message.ReplyToChannel("Ok...");
                    yield return message.ReplyToChannel(result.Data.Joke);
                }
                else
                {
                    yield return message.ReplyToChannel(string.Format("Dam, I can't think of one. [{0}]", result.StatusCode));
                }
            }
            else
            {
                foreach (ResponseMessage responseMessage in Next(message))
                {
                    yield return responseMessage;
                }
            }
        }

        protected override CommandDescription[] SupportedCommands()
        {
            return new []
            {
                new CommandDescription
                {
                    Command = "joke",
                    Description = "Tells a random joke"
                }
            };
        }

        private class JokeContainer
        {
            public string Joke { get; set; } 
        }
    }
}