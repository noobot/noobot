using System;
using System.IO;
using Noobot.Domain.Configuration;

namespace Noobot.Runner.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly IConfigReader _configReader;
        private TextWriterDistributor _distributor;

        public ConsoleLogger(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public void Grapple()
        {
            string logFile = Path.Combine(Environment.CurrentDirectory, _configReader.GetConfig().LogFile);

            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            var logStream = new FileStream(logFile, FileMode.OpenOrCreate, FileAccess.Write);
            var fileWriter = new StreamWriter(logStream) { AutoFlush = true };

            _distributor = new TextWriterDistributor(fileWriter, Console.Out);
            Console.SetOut(_distributor);
        }

        public void Dispose()
        {
            _distributor?.Dispose();
        }
    }
}