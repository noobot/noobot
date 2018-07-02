using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using System.IoC.StructureMapShim;
using Noobot.Console.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;

namespace Noobot.Console
{
    public class Program
    {
        private static INoobotCore _noobotCore;
        private static readonly SemaphoreSlim _quitEvent = new SemaphoreSlim(0,1);


        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Starting Noobot...");
            AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler; // closing the window doesn't hit this in Windows
            System.Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            await RunNoobot();
            await _quitEvent.WaitAsync();
        }
        
        private static async Task RunNoobot()
        {
            var registry = new SMRegistry();

            registry.Register<ILog>(GetLogger);
            registry.Register<IConfigReader, JsonConfigReader>();

            CompositionRoot<IConfigSpec, ILocatorConfigSpec, IRegSpec, IContainerSpec>
                .Compose(registry);

            var container = registry.GenerateContainer();
            _noobotCore = container.Resolve<INoobotCore>();

            await _noobotCore.Connect();
        }

        private static ConsoleOutLogger GetLogger()
        {
            return new ConsoleOutLogger("Noobot", LogLevel.All, true, true, false, "yyyy/MM/dd HH:mm:ss:fff");
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            // single threaded i hope
            if(_quitEvent.CurrentCount == 0)
                _quitEvent.Release();
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
