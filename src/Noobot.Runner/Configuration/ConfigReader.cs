using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Noobot.Core.Configuration;

namespace Noobot.Runner.Configuration
{
    public class ConfigReader : IConfigReader
    {
        public string SlackApiKey()
        {
            JObject jObject = GetJObject();
            return jObject.Value<string>("slack:apiToken");
        }

        public bool HelpEnabled()
        {
            return true;
        }

        public T GetConfigEntry<T>(string entryName)
        {
            JObject jObject = GetJObject();
            return jObject.Value<T>(entryName);
        }

        private JObject _currentJObject;
        private JObject GetJObject()
        {
            if (_currentJObject == null)
            {
                string fileName = Path.Combine(Environment.CurrentDirectory, @"configuration\config.json");
                string json = File.ReadAllText(fileName);
                _currentJObject = JObject.Parse(json);
            }

            return _currentJObject;
        }
    }
}