using Noobot.Domain.Configuration;

namespace Noobot.Runner
{
    public class NoobotHost : INoobotHost
    {
        private readonly IConfigReader _configReader;

        public NoobotHost(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }
    }
}