using Topshelf;

namespace Noobot.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<INoobotHost>(s =>
                {
                    s.ConstructUsing(name => new NoobotHost());
                    s.WhenStarted(n => n.Start());
                    s.WhenStopped(n => n.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDisplayName("Noobot");
                x.SetServiceName("Noobot");
                x.SetDescription("An extensible Slackbot built in C#");
            });
        }
    }
}
