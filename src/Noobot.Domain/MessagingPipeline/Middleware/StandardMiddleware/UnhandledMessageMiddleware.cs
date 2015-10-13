using System;
using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware
{
    /// <summary>
    /// Should always be the last middleware. Simply logs and stops the chain.
    /// </summary>
    internal class UnhandledMessageMiddleware : IMiddleware
    {
        public IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            Console.WriteLine("[{0}] Unhandled", message.MessageId);
            return new ResponseMessage[0];
        }

        public IEnumerable<CommandDescription> GetSupportedCommands()
        {
            return new CommandDescription[0];
        }
    }
}