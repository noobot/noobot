using Noobot.Runner.DependencyResolution;
using Noobot.Runner.Logging;
using Topshelf;

namespace Noobot.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var logger = Container.Instance.GetInstance<ILogger>())
            {
                logger.Grapple();

                HostFactory.Run(x =>
                {
                    x.Service<INoobotHost>(s =>
                    {
                        s.ConstructUsing(name => Container.Instance.GetInstance<INoobotHost>());
                        s.WhenStarted(n => n.Start());
                        s.WhenStopped(n =>
                        {
                            n.Stop();
                        });
                    });

                    x.RunAsLocalSystem();
                    x.SetDisplayName("Noobot");
                    x.SetServiceName("Noobot");
                    x.SetDescription("An extensible Slackbot built in C#");
                });
            }
        }
    }
}
