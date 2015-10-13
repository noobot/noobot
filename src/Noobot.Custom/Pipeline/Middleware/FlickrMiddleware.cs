using System.Collections.Generic;
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
            yield return message.ReplyToChannel("Ok, let's find you something about '{0}'", searchTerm);
            
        }
    }
}