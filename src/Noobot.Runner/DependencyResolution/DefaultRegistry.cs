using Noobot.Domain.Configuration;
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
                scan.Assembly("Noobot.Domain");
            });

            For<IConfigReader>()
                .Singleton();
        }
    }
}