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
        string SlackApiKey { get; }

        /// <summary>
        /// Should the "help" middleware be used?
        /// </summary>
        bool HelpEnabled { get; }

        /// <summary>
        /// Should the "stats" middleware be used?
        /// </summary>
        bool StatsEnabled { get; }

        /// <summary>
        /// Should the "about" middleware be used?
        /// </summary>
        bool AboutEnabled { get; }

        /// <summary>
        /// Should return any other configuration values you need within your middleware/plugins.
        /// </summary>
        T GetConfigEntry<T>(string entryName);
    }
}