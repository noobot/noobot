namespace Noobot.Core.MessagingPipeline.Response
{
    public class Attachment
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Fallback { get; set; }

        public string ImageUrl { get; set; }
        public string ThumbUrl { get; set; }
    }
}