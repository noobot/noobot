using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;

namespace Noobot.Console
{
    public class Program
    {
        private static INoobotCore _noobotCore;
        private static readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Starting Noobot...");
            AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler; // closing the window doesn't hit this in Windows
            System.Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            await RunNoobot();

            _quitEvent.WaitOne();
        }
        
        private static async Task RunNoobot()
        {
            var containerFactory = new ContainerFactory(
                new ConfigurationBase(),
                JsonConfigReader.DefaultLocation(),
                GetLogger());

            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            await _noobotCore.Connect();
        }

        private static ILogger GetLogger()
        {
            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                logging.AddConsole();
            });
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<ILogger<Program>>();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            _quitEvent.Set();
            consoleCancelEventArgs.Cancel = true;
        }

        // not hit
        private static void ProcessExitHandler(object sender, EventArgs e)
        {
            System.Console.WriteLine("Disconnecting...");
            _noobotCore?.Disconnect();
        }
    }
}
