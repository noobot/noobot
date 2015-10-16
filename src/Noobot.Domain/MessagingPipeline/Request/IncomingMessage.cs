using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Request
{
    public class IncomingMessage
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public string Channel { get; set; }
        public string UserChannel { get; set; }
        public bool BotIsMentioned { get; set; }
        public string BotName { get; set; }
        public string BotId { get; set; }


        public ResponseMessage ReplyToChannel(string format, params object[] values)
        {
            string text = string.Format(format, values);
            return ReplyToChannel(text);
        }

        public ResponseMessage ReplyToChannel(string text)
        {
            return ResponseMessage.ChannelMessage(Channel, text);
        }

        public ResponseMessage ReplyDirectlyToUser(string format, params object[] values)
        {
            string text = string.Format(format, values);
            return ReplyDirectlyToUser(text);
        }

        public ResponseMessage ReplyDirectlyToUser(string text)
        {
            return ResponseMessage.DirectUserMessage(UserChannel, UserId, text);
        }
    }
}