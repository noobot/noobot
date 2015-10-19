using System;
using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;

namespace Noobot.Domain.MessagingPipeline.Middleware
{
    public class HandlerMapping
    {
        public HandlerMapping()
        {
            ValidHandles = new string[0];
            MessageShouldTargetBot = true;
        }

        /// <summary>
        /// What the text should start with inorder to execute handler.
        /// </summary>
        public string[] ValidHandles { get; set; }

        /// <summary>
        /// Description of what this handle does. This appears in the "help" function.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// This is what code is run when a handle has been matched.
        /// </summary>
        public Func<IncomingMessage, string, IEnumerable<ResponseMessage>> EvaluatorFunc { get; set; }

        /// <summary>
        /// Defaults to "False". If set to True then the pipeline isn't interupted if a match occurs here.
        /// </summary>
        public bool ShouldContinueProcessing { get; set; }

        /// <summary>
        /// Defaults to "True". If set to false then any message is considered, even if it isn't targeted at the bot.
        /// </summary>
        public bool MessageShouldTargetBot { get; set; }
    }
}