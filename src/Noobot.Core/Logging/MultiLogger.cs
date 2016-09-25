using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Common.Logging.Factory;

namespace Noobot.Core.Logging
{
    /// <summary>
    /// Adapter hub for Common.Logging that can send logs to multiple other adapters
    /// http://stackoverflow.com/questions/11362410/common-logging-with-multiple-factory-adaptors
    /// </summary>
    public class MultiLogger : AbstractLogger
    {
        private readonly List<ILog> _loggers;

        public static readonly IDictionary<LogLevel, Action<ILog, object, Exception>> LogActions = new Dictionary<LogLevel, Action<ILog, object, Exception>>()
        {
            { LogLevel.Debug, (logger, message, exception) => logger.Debug(message, exception) },
            { LogLevel.Error, (logger, message, exception) => logger.Error(message, exception) },
            { LogLevel.Fatal, (logger, message, exception) => logger.Fatal(message, exception) },
            { LogLevel.Info, (logger, message, exception) => logger.Info(message, exception) },
            { LogLevel.Trace, (logger, message, exception) => logger.Trace(message, exception) },
            { LogLevel.Warn, (logger, message, exception) => logger.Warn(message, exception) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLogger"/> class.
        /// </summary>
        /// <param name="loggers">The loggers.</param>
        public MultiLogger(List<ILog> loggers)
        {
            _loggers = loggers;
        }

        public override bool IsDebugEnabled { get { return _loggers.Any(l => l.IsDebugEnabled); } }
        public override bool IsErrorEnabled { get { return _loggers.Any(l => l.IsErrorEnabled); } }
        public override bool IsFatalEnabled { get { return _loggers.Any(l => l.IsFatalEnabled); } }
        public override bool IsInfoEnabled { get { return _loggers.Any(l => l.IsInfoEnabled); } }
        public override bool IsTraceEnabled { get { return _loggers.Any(l => l.IsTraceEnabled); } }
        public override bool IsWarnEnabled { get { return _loggers.Any(l => l.IsWarnEnabled); } }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (var logger in _loggers)
            {
                try
                {
                    LogActions[level](logger, message, exception);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException("One or more exceptions occured while forwarding log message to multiple loggers", exceptions);
            }
        }
    }
}