namespace Noobot.Domain.Slack
{
    public class Message
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public string Channel { get; set; }
    }
}