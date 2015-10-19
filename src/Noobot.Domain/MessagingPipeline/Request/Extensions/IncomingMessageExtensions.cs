using System;
using System.Linq;

namespace Noobot.Domain.MessagingPipeline.Request.Extensions
{
    public static class IncomingMessageExtensions
    {
        public static string GetTargetedText(this IncomingMessage incomingMessage)
        {
            string formattedText = incomingMessage.FullText ?? string.Empty;

            string[] myNames =
            {
                incomingMessage.BotName + ":",
                incomingMessage.BotName,
                $"<@{incomingMessage.BotId}>:",
                $"<@{incomingMessage.BotId}>",
                $"@{incomingMessage.BotName}:",
                $"@{incomingMessage.BotName}",
            };

            string handle = myNames.FirstOrDefault(x => formattedText.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(handle))
            {
                formattedText = formattedText.Substring(handle.Length).Trim();
            }

            return formattedText;
        }
    }
}