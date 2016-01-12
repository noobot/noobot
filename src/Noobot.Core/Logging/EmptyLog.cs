namespace Noobot.Core.Logging
{
    internal class EmptyLog : ILog
    {
        public void Log(string data)
        { }
    }
}