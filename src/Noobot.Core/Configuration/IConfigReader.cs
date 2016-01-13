namespace Noobot.Core.Configuration
{
    /// <summary>
    /// A config reader is required to be supplied. This will probably vary by application so you will need to implement this.
    /// </summary>
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