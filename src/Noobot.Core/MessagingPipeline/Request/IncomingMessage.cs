using Noobot.Core.MessagingPipeline.Response;
using System.Collections.Generic;

namespace Noobot.Core.MessagingPipeline.Request
{
    public class IncomingMessage
    {
        /// <summary>
        /// The Slack UserId of whoever sent the message
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Username of whoever sent the mssage
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The channel used to send a DirectMessage back to the user who sent the message. 
        /// Note: this might be empty if the Bot hasn't talked to them privately before, but Noobot will join the DM automatically if required.
        /// </summary>
        public string UserChannel { get; set; }

        /// <summary>
        /// The email of the user that sent the message
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Contains the untainted raw Text that comes in from Slack. This hasn't been URL decoded.
        /// </summary>
        public string RawText { get; set; }

        /// <summary>
        /// Contains the URL decoded text from the message.
        /// </summary>
        public string FullText { get; set; }

        /// <summary>
        /// Contains the text minus any Bot targetting text (e.g. "@Noobot: {blah}" turns into "{blah}")
        /// </summary>
        public string TargetedText { get; set; }

        /// <summary>
        /// The 'channel' the message occured on. This might be a DirectMessage channel.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// The type of channel the message arrived on
        /// </summary>
        public ResponseType ChannelType { get; set; }

        /// <summary>
        /// Detects if the bot's name is mentioned anywhere in the text
        /// </summary>
        public bool BotIsMentioned { get; set; }

        /// <summary>
        /// The Bot's Slack name - this is configurable in Slack
        /// </summary>
        public string BotName { get; set; }

        /// <summary>
        /// The Bot's UserId
        /// </summary>
        public string BotId { get; set; }

        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public ResponseMessage ReplyToChannel(string text, Attachment attachment = null)
        {
            if (attachment == null)
                return ResponseMessage.ChannelMessage(Channel, text, attachments: null);
    
             var attachments = new List<Attachment> { attachment };
             return ReplyToChannel(text, attachments);
        }
        
        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public ResponseMessage ReplyToChannel(string text, List<Attachment> attachments)
        {
            return ResponseMessage.ChannelMessage(Channel, text, attachments);
        }

        /// <summary>
        /// Will send a DirectMessage reply to the use who sent the message
        /// </summary>
        public ResponseMessage ReplyDirectlyToUser(string text)
        {
            return ResponseMessage.DirectUserMessage(UserId, text);
        }

        /// <summary>
        /// Will display on Slack that the bot is typing on the current channel. Good for letting the end users know the bot is doing something.
        /// </summary>
        public ResponseMessage IndicateTypingOnChannel()
        {
            return ResponseMessage.ChannelMessage(Channel, string.Empty, attachments: null, message: new TypingIndicatorMessage());
        }

        /// <summary>
        /// Indicates on the DM channel that the bot is typing. Good for letting the end users know the bot is doing something.
        /// </summary>
        public ResponseMessage IndicateTypingOnDirectMessage()
        {
            return ResponseMessage.DirectUserMessage(UserChannel, UserId, string.Empty, new TypingIndicatorMessage());
        }
    }
}