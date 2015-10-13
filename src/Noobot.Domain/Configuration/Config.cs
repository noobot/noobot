namespace Noobot.Domain.Configuration
{
    public class Config
    {
        public string LogFile { get; set; }
        public SlackConfig Slack { get; set; }
        public FlickrConfig Flickr { get; set; }
    }
}