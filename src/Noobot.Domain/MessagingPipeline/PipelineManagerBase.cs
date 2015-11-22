using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Middleware.StandardMiddleware;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace Noobot.Domain.MessagingPipeline
{
    public abstract class PipelineManagerBase : IPipelineManager
    {
        private readonly Stack<Type> _pipeline = new Stack<Type>();

        protected abstract void Initialise();
        public Registry Initialise(Registry registry)
        {
            Initialise();

            registry.Scan(x =>
            {
                // scan assemblies that we are loading pipelines from
                foreach (Type middlewareType in _pipeline)
                {
                    x.AssemblyContainingType(middlewareType);
                }
            });

            registry.For<IMiddleware>().Use<UnhandledMessageMiddleware>();

            if (_pipeline.Any())
            {
                while (_pipeline.Any())
                {
                    Type nextType = _pipeline.Pop();
                    var nextDeclare = registry.For<IMiddleware>();

                    MethodInfo decorateMethod = nextDeclare.GetType().GetMethod("DecorateAllWith", new[] { typeof(Func<Instance, bool>) });
                    MethodInfo generic = decorateMethod.MakeGenericMethod(nextType);
                    generic.Invoke(nextDeclare, new object[] { null });
                }
            }

            registry.For<IMiddleware>().DecorateAllWith<AboutMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<ScheduleMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<HelpMiddleware>();
            registry.For<IMiddleware>().DecorateAllWith<BeginMessageMiddleware>();

            return registry;
        }
        
        protected void Use<T>() where T : IMiddleware
        {
            _pipeline.Push(typeof(T));
        }
    }
}