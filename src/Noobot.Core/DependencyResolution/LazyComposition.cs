using System.IoC;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noobot.Core.DependencyResolution
{
    internal class LazyComposition
    {
        readonly Lazy<IMiddleware> middleware;
        readonly Lazy<IPlugin[]> plugins;

        public IMiddleware Middleware => middleware.Value;
        public IPlugin[] Plugins => plugins.Value;

        public LazyComposition(IGenericContainer container)
        {
            middleware = new Lazy<IMiddleware>(() => container.Resolve<IMiddleware>());
            plugins = new Lazy<IPlugin[]>(() => container.Resolve<IPlugin[]>());
        }
    }
}