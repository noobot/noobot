using System;
using System.Collections.Generic;

namespace Noobot.Core.MessagingPipeline
{
    public interface IPipelineConfiguration
    {
        List<Type> ListMiddlewareTypes();
    }
}