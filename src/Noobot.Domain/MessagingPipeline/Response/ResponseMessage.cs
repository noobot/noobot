namespace Noobot.Domain.MessagingPipeline.Response
{
    public enum ResponseMessageType
    {
        Channel,
        DirectMessage
    }

    public class ResponseMessage
    {
        public string Text { get; set; }
        public string Channel { get; set; }
        public ResponseMessageType MessageType { get; set; }
    }
}