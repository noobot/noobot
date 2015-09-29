using System;
using System.IO;
using Newtonsoft.Json;

namespace Noobot.Domain.Configuration
{
    public class ConfigReader : IConfigReader
    {
        private Config Current { get; set; }

        public Config GetConfig()
        {
            if (Current == null)
            {
                string fileName = Path.Combine(Environment.CurrentDirectory, @"configuration\config.json");
                string json = System.IO.File.ReadAllText(fileName);
                Current = JsonConvert.DeserializeObject<Config>(json);
            }

            return Current;
        }
    }
}