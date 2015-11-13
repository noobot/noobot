using System;
using System.IO;
using Newtonsoft.Json;

namespace Noobot.Domain.Configuration
{
    public class ConfigReader : IConfigReader
    {
        private dynamic Current { get; set; }

        public dynamic GetConfig()
        {
            if (Current == null)
            {
                string fileName = Path.Combine(Environment.CurrentDirectory, @"configuration\config.json");
                string json = File.ReadAllText(fileName);
                Current = JsonConvert.DeserializeObject<dynamic>(json);
            }

            return Current;
        }
    }
}