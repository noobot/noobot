namespace Noobot.Core.MessagingPipeline.Middleware.ValidHandles
{
    public interface IValidHandle
    {
        bool IsMatch(string message);
        string HandleHelpText { get; }
    }
}