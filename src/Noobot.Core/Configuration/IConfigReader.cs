using System;
using Newtonsoft.Json.Linq;

namespace Noobot.Core.Configuration
{
    public interface IConfigReader
    {
        string SlackApiKey();
        T GetConfigEntry<T>(string entryName);
    }
}