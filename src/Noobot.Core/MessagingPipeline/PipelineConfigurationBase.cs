using System;
using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;

namespace Noobot.Core.MessagingPipeline
{
    public abstract class PipelineConfigurationBase : IPipelineConfiguration
    {
        private readonly List<Type> _pipeline = new List<Type>();

        public List<Type> ListMiddlewareTypes()
        {
            return _pipeline;
        }

        protected void Use<T>() where T : IMiddleware
        {
            _pipeline.Add(typeof(T));
        }
    }
}