using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Noobot.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Console.WriteLine($"Noobot assembly version: {Assembly.GetExecutingAssembly().GetName().Version}");

            HostFactory.Run(x =>
            {
                x.Service<NoobotHost>(s =>
                {
                    // EXAMPLE: Simple construction using default values:
                    //s.ConstructUsing(_ => new NoobotHost());

                    // EXAMPLE: Construction with customised values:
                    //s.ConstructUsing(_ => new NoobotHost(new ConfigReader(@"configuration\config.json")
                    //{
                    //    AboutEnabled = true,
                    //    HelpEnabled = true,
                    //    StatsEnabled = true
                    //}));

                    s.ConstructUsing(_ => new NoobotHost());
                    s.WhenStarted(n => n.Start());
                    s.WhenStopped(n => n.Stop());
                });

                x.RunAsNetworkService();
                x.SetDisplayName("Noobot");
                x.SetServiceName("Noobot");
                x.SetDescription("An extensible Slackbot built in C#");
            });
        }
    }
}
