using System;

namespace Noobot.Core.MessagingPipeline.Middleware.ValidHandles
{
    public class StartsWithHandle : IValidHandle
    {
        private readonly string _messageStartsWith;

        public StartsWithHandle(string messageStartsWith)
        {
            _messageStartsWith = messageStartsWith ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return (message ?? string.Empty).StartsWith(_messageStartsWith, StringComparison.InvariantCultureIgnoreCase);
        }

        public string HandleHelpText => _messageStartsWith;
    }
}