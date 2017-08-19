using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Noobot.Console.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;

namespace Noobot.Console
{
    public class Program
    {
        private static INoobotCore _noobotCore;
        private static readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting Noobot...");
            AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler; // closing the window doesn't hit this in Windows
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            System.Console.CancelKeyPress += ConsoleOnCancelKeyPress;


            RunNoobot()
                .GetAwaiter()
                .GetResult();

            _quitEvent.WaitOne();
        }
        
        private static async Task RunNoobot()
        {
            var containerFactory = new ContainerFactory(
                new ConfigurationBase(),
                new ConfigReader(),
                new ConsoleOutLogger("Noobot", LogLevel.All, true, true, false, "yyyy/MM/dd HH:mm:ss:fff"));

            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            await _noobotCore.Connect();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Close();
        }

        // not hit
        private static void ProcessExitHandler(object sender, EventArgs e)
        {
            Close();
        }

        // not hit
        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private static void Close()
        {
            System.Console.WriteLine("Disconnecting...");
            _noobotCore?.Disconnect();
            _quitEvent.Set();
        }
    }
}
