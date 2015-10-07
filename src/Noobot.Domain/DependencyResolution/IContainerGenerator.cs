using Noobot.Domain.Slack;

namespace Noobot.Domain.DependencyResolution
{
    public interface IContainerGenerator
    {
        INoobotContainer Generate();
    }
}