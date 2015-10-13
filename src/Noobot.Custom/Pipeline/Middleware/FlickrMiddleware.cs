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
        private readonly FlickrConfig _flickrConfig;

        public FlickrMiddleware(IMiddleware next, IConfigReader configReader) : base(next)
        {
            _flickrConfig = configReader.GetConfig().Flickr;

            HandlerMappings = new []
            {
                new HandlerMapping
                {
                    ValidHandles = new [] { "/flickr", "flickr", "/pic", "pic"},
                    Description = "Finds a pics from flickr - usage: /flickr birds",
                    EvaluatorFunc = FlickrHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> FlickrHandler(IncomingMessage message, string matchedHandle)
        {
            string searchTerm = message.Text.Substring(matchedHandle.Length).Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                yield return message.ReplyToChannel("Please give me something to search, e.g. {0} trains", matchedHandle);
            }
            else
            {
                yield return message.ReplyToChannel("Ok, let's find you something about '{0}'", searchTerm);

                var flickr = new Flickr(_flickrConfig.ApiKey);
                var options = new PhotoSearchOptions { Tags = searchTerm, PerPage = 1, Page = 1 };
                PhotoCollection photos = flickr.PhotosSearch(options);

                yield return message.ReplyToChannel(photos.First().LargeUrl);
            }
            
        }
    }
}