namespace Noobot.Core.Configuration
{
    public interface IConfigReader
    {
        string SlackApiKey();
        bool HelpEnabled();
        T GetConfigEntry<T>(string entryName);
    }
}