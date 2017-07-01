using System;
using System.IO;
using System.Reflection;
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

        public bool StatsEnabled { get; } = true;
        public bool AboutEnabled { get; } = true;
        public bool HelpEnabled { get; } = true;

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
                string assemblyLocation = AssemblyLocation();
                string fileName = Path.Combine(assemblyLocation, @"configuration\config.json");
                string json = File.ReadAllText(fileName);
                _currentJObject = JObject.Parse(json);
            }

            return _currentJObject;
        }

        private string AssemblyLocation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var path = Path.GetDirectoryName(codebase.LocalPath);
            return path;
        }
    }
}