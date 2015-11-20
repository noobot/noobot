using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
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
            HandlerMappings = new[]
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
            yield return message.IndicateTypingOnChannel();

            IRestResponse jokeResponse = new Random().Next(0, 100) < 70 ? GetTambalJoke() : GetMommaJoke();
            if (jokeResponse.StatusCode == HttpStatusCode.OK)
            {
                var joke = JsonConvert.DeserializeObject<JokeContainer>(jokeResponse.Content);
                
                yield return message.ReplyToChannel(joke.Joke);
            }
            else
            {
                yield return message.ReplyToChannel($"Dam, I can't think of one. [{jokeResponse.StatusCode}]");
            }
        }

        private IRestResponse GetTambalJoke()
        {
            var client = new RestClient("http://tambal.azurewebsites.net");
            var request = new RestRequest("/joke/random", Method.GET);
            return client.Execute(request);
        }

        private IRestResponse GetMommaJoke()
        {
            var client = new RestClient("http://api.yomomma.info");
            var request = new RestRequest("/", Method.GET);
            return client.Execute(request);
        }

        private class JokeContainer
        {
            public string Joke { get; set; }
        }
    }
}