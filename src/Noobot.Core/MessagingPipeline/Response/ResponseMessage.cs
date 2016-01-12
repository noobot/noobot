namespace Noobot.Core.MessagingPipeline.Response
{
    public class ResponseMessage
    {
        public string Text { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }
        public ResponseType ResponseType { get; set; }
        public Attachment Attachment { get; set; }

        public static ResponseMessage DirectUserMessage(string userId, string text, ResponseMessage message = null)
        {
            return DirectUserMessage(string.Empty, userId, text, message);
        }

        public static ResponseMessage DirectUserMessage(string userChannel, string userId, string text, ResponseMessage message = null)
        {
            if(message == null)
                message = new ResponseMessage();

            message.Channel = userChannel;
            message.ResponseType = ResponseType.DirectMessage;
            message.UserId = userId;
            message.Text = text;

            return message;
        }

        public static ResponseMessage ChannelMessage(string channel, string text, Attachment attachment, ResponseMessage message = null)
        {
            if (message == null)
                message = new ResponseMessage();

            message.Channel = channel;
            message.ResponseType = ResponseType.Channel;
            message.Text = text;
            message.Attachment = attachment;

            return message;
        }
    }
}