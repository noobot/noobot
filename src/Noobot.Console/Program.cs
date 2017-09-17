using System;
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
                new JsonConfigReader(),
                GetLogger());

            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            await _noobotCore.Connect();
        }

        private static ConsoleOutLogger GetLogger()
        {
            return new ConsoleOutLogger("Noobot", LogLevel.All, true, true, false, "yyyy/MM/dd HH:mm:ss:fff");
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
