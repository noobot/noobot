using System;
using System.IO;
using System.Reflection;
using Noobot.Core.Configuration;
using Noobot.Runner.Configuration;
using Topshelf;

namespace Noobot.Runner
{
    public class Program
    {
        private static readonly IConfigReader ConfigReader = new ConfigReader();

        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            Console.WriteLine($"Noobot assembly version: {Assembly.GetExecutingAssembly().GetName().Version}");

            HostFactory.Run(x =>
            {
                x.Service<NoobotHost>(s =>
                {
                    s.ConstructUsing(name => new NoobotHost(ConfigReader));

                    s.WhenStarted(n =>
                    {
                        n.Start();
                    });

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
