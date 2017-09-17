using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;

namespace Noobot.Console.Logging
{
    public class ConsoleOutLogger : AbstractSimpleLogger
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> colors = new Dictionary<LogLevel, ConsoleColor>
        {
            { LogLevel.Fatal, ConsoleColor.Red },
            { LogLevel.Error, ConsoleColor.Yellow },
            { LogLevel.Warn, ConsoleColor.Magenta },
            { LogLevel.Info, ConsoleColor.White },
            { LogLevel.Debug, ConsoleColor.Gray },
            { LogLevel.Trace, ConsoleColor.DarkGray },
        };

        private readonly bool useColor;

        /// <summary>
        /// Creates and initializes a logger that writes messages to <see cref="Noobot.Console.Out" />.
        /// </summary>
        /// <param name="logName">The name, usually type name of the calling class, of the logger.</param>
        /// <param name="logLevel">The current logging threshold. Messages recieved that are beneath this threshold will not be logged.</param>
        /// <param name="showLevel">Include the current log level in the log message.</param>
        /// <param name="showDateTime">Include the current time in the log message.</param>
        /// <param name="showLogName">Include the instance name in the log message.</param>
        /// <param name="dateTimeFormat">The date and time format to use in the log message.</param>
        public ConsoleOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
            : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
        }

        /// <summary>
        /// Creates and initializes a logger that writes messages to <see cref="Noobot.Console.Out" />.
        /// </summary>
        /// <param name="logName">The name, usually type name of the calling class, of the logger.</param>
        /// <param name="logLevel">The current logging threshold. Messages recieved that are beneath this threshold will not be logged.</param>
        /// <param name="showLevel">Include the current log level in the log message.</param>
        /// <param name="showDateTime">Include the current time in the log message.</param>
        /// <param name="showLogName">Include the instance name in the log message.</param>
        /// <param name="dateTimeFormat">The date and time format to use in the log message.</param>
        /// <param name="useColor">Use color when writing the log message.</param>
        public ConsoleOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat, bool useColor)
            : this(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
            this.useColor = useColor;
        }

        /// <summary>
        /// Do the actual logging by constructing the log message using a <see cref="StringBuilder" /> then
        /// sending the output to <see cref="Noobot.Console.Out" />.
        /// </summary>
        /// <param name="level">The <see cref="LogLevel" /> of the message.</param>
        /// <param name="message">The log message.</param>
        /// <param name="e">An optional <see cref="Exception" /> associated with the message.</param>
        protected override void WriteInternal(LogLevel level, object message, Exception e)
        {
            // Use a StringBuilder for better performance
            StringBuilder sb = new StringBuilder();
            FormatOutput(sb, level, message, e);

            // Print to the appropriate destination
            ConsoleColor color;
            if (this.useColor && colors.TryGetValue(level, out color))
            {
                var originalColor = System.Console.ForegroundColor;
                try
                {
                    System.Console.ForegroundColor = color;
                    System.Console.Out.WriteLine(sb.ToString());
                    return;
                }
                finally
                {
                    System.Console.ForegroundColor = originalColor;
                }
            }

            System.Console.Out.WriteLine(sb.ToString());
        }
    }
}