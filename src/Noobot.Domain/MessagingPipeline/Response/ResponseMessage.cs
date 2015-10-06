namespace Noobot.Domain.MessagingPipeline.Response
{
    public class ResponseMessage
    {
        public string Text { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }
        public ResponseType ResponseType { get; set; }
    }
}