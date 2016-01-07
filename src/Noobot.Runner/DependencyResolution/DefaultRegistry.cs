using Noobot.Core.Logging;
using Noobot.Core.MessagingPipeline;
using Noobot.Custom;
using Noobot.Runner.Logging;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Noobot.Runner.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
                scan.Assembly("Noobot.Core");
                scan.Assembly("Noobot.Custom");
            });

            For<INoobotHost>()
                .Use<NoobotHost>();
            
            For<IPipelineConfiguration>()
                .Use<PipelineConfiguration>();

            For<ILogger>()
                .Use<ConsoleLogger>();

            For<ILog>()
                .Use<ConsoleLog>();
        }
    }
}