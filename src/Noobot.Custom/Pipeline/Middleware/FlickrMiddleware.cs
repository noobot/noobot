using System;
using System.Collections.Generic;
using System.Linq;
using FlickrNet;
using Noobot.Domain.Configuration;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class FlickrMiddleware : MiddlewareBase
    {
        private readonly IConfigReader _configReader;

        public FlickrMiddleware(IMiddleware next, IConfigReader configReader) : base(next)
        {
            _configReader = configReader;

            HandlerMappings = new []
            {
                new HandlerMapping
                {
                    ValidHandles = new [] { "/flickr", "flickr", "/pic", "pic"},
                    Description = "Finds a pics from flickr - usage: /flickr birds",
                    EvaluatorFunc = FlickrHandler,
                }
            };
        }

        private IEnumerable<ResponseMessage> FlickrHandler(IncomingMessage message, string matchedHandle)
        {
            string searchTerm = message.TargetedText.Substring(matchedHandle.Length).Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                yield return message.ReplyToChannel($"Please give me something to search, e.g. {matchedHandle} trains");
            }
            else
            {
                yield return message.ReplyToChannel($"Ok, let's find you something about '{searchTerm}'");
                string apiKey = _configReader.GetConfig().Flickr?.ApiKey;

                if (string.IsNullOrEmpty(apiKey))
                {
                    yield return message.ReplyToChannel("Woops, looks like a Flickr API Key has not been entered. Please ask the admin to fix this");
                }
                else
                {
                    var flickr = new Flickr(apiKey);

                    var options = new PhotoSearchOptions {Text = searchTerm, PerPage = 100, Page = 1};
                    PhotoCollection photos = flickr.PhotosSearch(options);

                    if (photos.Any())
                    {
                        int i = new Random().Next(0, photos.Count);
                        Photo photo = photos[i];
                        yield return message.ReplyToChannel(photo.LargeUrl);
                    }
                    else
                    {
                        yield return message.ReplyToChannel($"Sorry @{message.Username}, I couldn't find anything about {searchTerm}");
                    }
                }
            }
        }
    }
}