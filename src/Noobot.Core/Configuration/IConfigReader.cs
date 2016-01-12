namespace Noobot.Core.Configuration
{
    public interface IConfigReader
    {
        /// <summary>
        /// Should return the API key required to connect to your team.
        /// </summary>
        string SlackApiKey();

        /// <summary>
        /// Should the "help" middleware be used?
        /// </summary>
        bool HelpEnabled();

        /// <summary>
        /// Should return any other configuration values you need within your middleware/plugins.
        /// </summary>
        T GetConfigEntry<T>(string entryName);
    }
}