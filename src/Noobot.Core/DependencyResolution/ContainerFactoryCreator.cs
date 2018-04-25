using Common.Logging;
using LetsAgree.IOC;
using Noobot.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noobot.Core.DependencyResolution
{
    public static class ContainerFactoryCreator
    {
        public static IContainerFactory Create<C, R, T>(IConfiguration configuration, IConfigReader configReader, IRegistryCreator<C, R, T> registryCreator, ILog logger = null)
            where R : IBasicRegistration<C, T>, IGenericRegistration<C, T>, IScanningRegistraction<C, T>
            where C : ISingletonConfig, IDecoratorConfig
            where T : IBasicContainer, IGenericContainer
        {
            return new ContainerFactory<C, R, T>(configuration, configReader, registryCreator, logger);
        }
    }
}
