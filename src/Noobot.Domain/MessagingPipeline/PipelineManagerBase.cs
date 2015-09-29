using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace Noobot.Domain.MessagingPipeline
{
    public abstract class PipelineManagerBase : IPipelineManager
    {
        private readonly Stack<Type> _pipeline = new Stack<Type>();
        private Container Container { get; set; }

        public abstract void Initialise();

        public IMiddleware GetPipeline()
        {
            if (Container == null)
            {
                Container = CreateContainer();
            }

            return Container.GetInstance<IMiddleware>();
        }

        private Container CreateContainer()
        {
            var registry = new Registry();
            registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
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

            return new Container(registry);
        }

        protected void Use<T>() where T : IMiddleware
        {
            _pipeline.Push(typeof(T));
        }
    }
}