using Noobot.Domain.DependencyResolution;
using Noobot.Domain.MessagingPipeline.Middleware;

namespace Noobot.Domain.MessagingPipeline
{
    public class PipelineFactory : IPipelineFactory
    {
        private INoobotContainer _noobotContainer = null;

        public void SetContainer(INoobotContainer noobotContainer)
        {
            _noobotContainer = noobotContainer;
        }

        public IMiddleware GetPipeline()
        {
            return _noobotContainer.GetInstance<IMiddleware>();
        }
    }
}