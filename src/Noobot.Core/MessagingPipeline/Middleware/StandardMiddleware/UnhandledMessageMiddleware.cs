using System;
using System.Collections.Generic;
using Noobot.Core.Logging;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware
{
    /// <summary>
    /// Should always be the last middleware. Simply logs and stops the chain.
    /// </summary>
    internal class UnhandledMessageMiddleware : IMiddleware
    {
        private readonly ILog _log;

        public UnhandledMessageMiddleware(ILog log)
        {
            _log = log;
        }

        public IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            _log.Log("Unhandled");
            return new ResponseMessage[0];
        }

        public IEnumerable<CommandDescription> GetSupportedCommands()
        {
            return new CommandDescription[0];
        }
    }
}