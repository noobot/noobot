using System;
using System.Collections.Generic;
using Noobot.Core.MessagingPipeline;
using Registry = StructureMap.Configuration.DSL.Registry;

namespace Noobot.Tests.Unit.Stubs.MessagingPipeline
{
    public class PipelineConfigurationStub : IPipelineConfiguration
    {
        public Registry Initialise(Registry registry)
        {
            throw new System.NotImplementedException();
        }

        public List<Type> ListMiddlewareTypes()
        {
            throw new NotImplementedException();
        }
    }
}