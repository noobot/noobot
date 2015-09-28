using System;
using System.IO;
using Newtonsoft.Json;

namespace Noobot.Domain.Configuration
{
    public class Config
    {
        public SlackConfig Slack { get; set; }



        private static Config Current { get; set; }
        public static Config GetConfig()
        {
            if (Current == null)
            {
                string fileName = Path.Combine(Environment.CurrentDirectory, "config.json");
                string json = System.IO.File.ReadAllText(fileName);
                Current = JsonConvert.DeserializeObject<Config>(json);
            }

            return Current;
        }
    }
}