using System;
using Common.Logging;
using Common.Logging.Simple;

namespace Noobot.Core.Logging
{
    public class EmptyLogger : AbstractSimpleLogger
    {
        public EmptyLogger() : base(null, LogLevel.All, false, false, false, null)
        { }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {

        }
    }
}