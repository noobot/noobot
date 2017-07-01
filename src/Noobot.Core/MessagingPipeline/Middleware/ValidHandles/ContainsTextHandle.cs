using System;

namespace Noobot.Core.MessagingPipeline.Middleware.ValidHandles
{
    public class ContainsTextHandle : IValidHandle
    {
        private readonly string _containsText;

        public ContainsTextHandle(string containsText)
        {
            _containsText = containsText ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return (message ?? string.Empty).IndexOf(_containsText, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public string HandleHelpText => _containsText;
    }
}