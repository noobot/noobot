using System;
using System.Linq;

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
            return (message ?? string.Empty).IndexOf(_containsText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public string HandleHelpText => _containsText;

        public static IValidHandle[] For(params string[] containsText)
        {
            return containsText
                .Select(x => new ContainsTextHandle(x))
                .Cast<IValidHandle>()
                .ToArray();
        }
    }
}