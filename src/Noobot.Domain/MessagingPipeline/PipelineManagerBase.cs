using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noobot.Domain.MessagingPipeline.Middleware;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
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
                MethodInfo method = x.GetType().GetMethod("AssemblyContainingType", new Type[0]);
                foreach (Type middlewareType in _pipeline)
                {
                    MethodInfo generic = method.MakeGenericMethod(middlewareType);
                    generic.Invoke(x, null);
                }
            });

            if (_pipeline.Any())
            {
                Type firstType = _pipeline.Pop();
                var firstDeclare = registry.For<IMiddleware>();

                MethodInfo method = firstDeclare.GetType().GetMethod("Use", new Type[0]);
                MethodInfo generic = method.MakeGenericMethod(firstType);
                generic.Invoke(firstDeclare, null);

                while (_pipeline.Any())
                {
                    var nextType = _pipeline.Pop();
                    var nextDeclare = registry.For<IMiddleware>();

                    MethodInfo decorateMethod = nextDeclare.GetType().GetMethod("DecorateAllWith", new[] { typeof(Func<Instance, bool>) });
                    generic = decorateMethod.MakeGenericMethod(nextType);
                    generic.Invoke(nextDeclare, new object[] { null });
                }
            }

            return registry;
        }
        
        protected void Use<T>() where T : IMiddleware
        {
            _pipeline.Push(typeof(T));
        }
    }
}