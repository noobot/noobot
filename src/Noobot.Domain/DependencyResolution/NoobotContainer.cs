using Noobot.Domain.Slack;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Noobot.Domain.DependencyResolution
{
    public class NoobotContainer : Container, INoobotContainer
    {
        public NoobotContainer(Registry registry) : base(registry)
        { }

        public ISlackConnector GetSlackConnector()
        {
            return GetInstance<ISlackConnector>();
        }
    }
}