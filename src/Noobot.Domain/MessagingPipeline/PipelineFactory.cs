using Noobot.Core.DependencyResolution;
using Noobot.Core.MessagingPipeline.Middleware;

namespace Noobot.Core.MessagingPipeline
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