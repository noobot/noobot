using System.Linq;
using System.Text.RegularExpressions;

namespace Noobot.Core.MessagingPipeline.Middleware.ValidHandles
{
    public class RegexHandle : IValidHandle
    {
        private readonly Regex _regex;

        public RegexHandle(string regexPattern, RegexOptions regexOptions = RegexOptions.IgnoreCase)
            : this(regexPattern, regexPattern, regexOptions)
        { }

        public RegexHandle(string regexPattern, string helpText, RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            _regex = new Regex(regexPattern ?? string.Empty, regexOptions);
            HandleHelpText = helpText ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return _regex.IsMatch(message ?? string.Empty);
        }

        public string HandleHelpText { get; }

        public static IValidHandle[] For(params string[] regexPatterns)
        {
            return regexPatterns
                .Select(x => new RegexHandle(x))
                .Cast<IValidHandle>()
                .ToArray();
        }
    }
}