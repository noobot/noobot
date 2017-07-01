namespace Noobot.Core.MessagingPipeline.Middleware.ValidHandles
{
    public class AlwaysMatchHandle : IValidHandle
    {
        public bool IsMatch(string message)
        {
            return true;
        }

        public string HandleHelpText => "<anything>";
    }
}