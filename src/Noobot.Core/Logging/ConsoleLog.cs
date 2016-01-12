using System;

namespace Noobot.Core.Logging
{
    public class ConsoleLog : ILog
    {
        public void Log(string data)
        {
            Console.WriteLine(data);
        }
    }
}