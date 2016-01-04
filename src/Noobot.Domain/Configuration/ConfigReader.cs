using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Noobot.Core.Configuration
{
    public class ConfigReader : IConfigReader
    {
        private JObject Current { get; set; }

        public JObject GetConfig()
        {
            if (Current == null)
            {
                string fileName = Path.Combine(Environment.CurrentDirectory, @"configuration\config.json");
                string json = File.ReadAllText(fileName);
                Current = JObject.Parse(json);
            }

            return Current;
        }
    }
}