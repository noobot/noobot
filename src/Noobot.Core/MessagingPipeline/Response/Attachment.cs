using System.Collections.Generic;

namespace Noobot.Core.MessagingPipeline.Response
{
    public class Attachment
    {
        public Attachment()
        {
            AttachmentFields = new List<AttachmentField>();
        }

        public string Text { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Fallback { get; set; }

        public string ImageUrl { get; set; }
        public string ThumbUrl { get; set; }

        public string Color { get; set; }

        public List<AttachmentField> AttachmentFields { get; set; }

        public Attachment AddAttachmentField(string title, string value)
        {
            return AddAttachmentField(title, value, false);
        }

        public Attachment AddAttachmentField(string title, string value, bool isShort)
        {
            AttachmentFields.Add(new AttachmentField
            {
                Title = title,
                Value = value,
                IsShort = isShort
            });

            return this;
        }
    }
}