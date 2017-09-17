using System.Collections.Generic;
using SlackConnector.Models;

namespace Noobot.Core.MessagingPipeline.Response
{
    public class ResponseMessage
    {
        public string Text { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }
        public ResponseType ResponseType { get; set; }
        public List<SlackAttachment> Attachments { get; set; }

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

        public static ResponseMessage ChannelMessage(string channel, string text, SlackAttachment attachment, ResponseMessage message = null)
        {
            List<SlackAttachment> attachments = null;
            if (attachment != null)
                attachments = new List<SlackAttachment> { attachment };

            return ChannelMessage(channel, text, attachments, message);
        }

        public static ResponseMessage ChannelMessage(string channel, string text, List<SlackAttachment> attachments, ResponseMessage message = null)
        {
            if (message == null)
                message = new ResponseMessage();

            message.Channel = channel;
            message.ResponseType = ResponseType.Channel;
            message.Text = text;
            message.Attachments = attachments;

            return message;
        }
    }
}