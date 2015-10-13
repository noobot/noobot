using System.Collections.Generic;
using System.Net;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using RestSharp;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class JokeMiddleware : MiddlewareBase
    {
        public JokeMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new []
            {
                new HandlerMapping
                {
                    ValidHandles = new [] { "/joke", "joke", "tell me a joke"},
                    Description = "Tells a random joke",
                    EvaluatorFunc = JokeHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> JokeHandler(IncomingMessage message, string matchedHandle)
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

        private class JokeContainer
        {
            public string Joke { get; set; } 
        }
    }
}