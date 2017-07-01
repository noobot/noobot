using System;
using System.Linq;

namespace Noobot.Core.MessagingPipeline.Middleware.ValidHandles
{
    public class ExactMatchHandle : IValidHandle
    {
        private readonly string _messageToMatch;

        public ExactMatchHandle(string messageToMatch)
        {
            _messageToMatch = messageToMatch ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return (message ?? string.Empty).Equals(_messageToMatch, StringComparison.InvariantCultureIgnoreCase);
        }

        public string HandleHelpText => _messageToMatch;

        public static IValidHandle[] For(params string[] messagesToMatch)
        {
            return messagesToMatch
                .Select(x => new ExactMatchHandle(x))
                .Cast<IValidHandle>()
                .ToArray();
        }
    }
}