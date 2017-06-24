using System;
using System.Collections.Generic;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

using RestSharp;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class JokeMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public JokeMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
            HandlerMappings = new[]
                              {
                                  new HandlerMapping
                                  {
                                      ValidHandles = new[] {"joke", "tell me a joke"},
                                      Description = "Tells a random joke",
                                      EvaluatorFunc = JokeHandler
                                  }
                              };
        }

        private IEnumerable<ResponseMessage> JokeHandler(IncomingMessage message, string matchedHandle)
        {
            yield return message.IndicateTypingOnChannel();

            IRestResponse jokeResponse = new Random().Next(0, 9) % 2 == 0 ? GetChuckNorrisJoke() : GetMommaJoke();
            if (jokeResponse.StatusCode == HttpStatusCode.OK)
            {
                _statsPlugin.IncrementState("Jokes:Told");
                var jokeObject = JObject.Parse(jokeResponse.Content);
                var jokeString = $"{{ {jokeObject.SelectToken("$..joke").Parent} }}";
                var joke = JsonConvert.DeserializeObject<JokeContainer>(jokeString);

                yield return message.ReplyToChannel(joke.Joke);
            }
            else
            {
                _statsPlugin.IncrementState("Jokes:Failed");
                yield return message.ReplyToChannel($"Damn, I can't think of one. [{jokeResponse.StatusCode}]");
            }
        }

        private IRestResponse GetChuckNorrisJoke()
        {
            var client = new RestClient("http://api.icndb.com");
            var request = new RestRequest("/jokes/random", Method.GET);
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
            [JsonProperty("joke", Required = Required.Always)]
            public string Joke { get; set; }
        }
    }
}