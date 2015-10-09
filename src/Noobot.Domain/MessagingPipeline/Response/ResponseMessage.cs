namespace Noobot.Domain.MessagingPipeline.Response
{
    public class ResponseMessage
    {
        public string Text { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }
        public ResponseType ResponseType { get; set; }

        public static ResponseMessage DirectUserMessage(string userId, string text)
        {
            return DirectUserMessage(string.Empty, userId, text);
        }

        public static ResponseMessage DirectUserMessage(string userChannel, string userId, string text)
        {
            return new ResponseMessage
            {
                Channel = userChannel,
                ResponseType = ResponseType.DirectMessage,
                UserId = userId,
                Text = text
            };
        }

        public static ResponseMessage ChannelMessage(string channel, string text)
        {
            return new ResponseMessage
            {
                Channel = channel,
                ResponseType = ResponseType.Channel,
                Text = text
            };
        }
    }
}