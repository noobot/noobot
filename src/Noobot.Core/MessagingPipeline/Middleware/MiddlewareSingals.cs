using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.MessagingPipeline.Middleware
{
    public static class MiddlewareSingals
    {
        public static readonly ResponseMessage StopProcessing = new ResponseMessage();
    }
}