using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Factory;

namespace Noobot.Core.Logging
{
    /// <summary>
    /// Adapter hub for Common.Logging that can send logs to multiple other adapters
    /// http://stackoverflow.com/questions/11362410/common-logging-with-multiple-factory-adaptors
    /// </summary>
    public class MultiLoggerFactoryAdapter : AbstractCachingLoggerFactoryAdapter
    {
        private readonly List<ILoggerFactoryAdapter> _loggerFactoryAdapters;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLoggerFactoryAdapter"/> class.
        /// </summary>
        public MultiLoggerFactoryAdapter(NameValueCollection properties)
        {
            const string adapterString = ".factoryAdapter";
            _loggerFactoryAdapters = new List<ILoggerFactoryAdapter>();

            foreach (var factoryAdapter in properties.Where(e => e.Key.EndsWith(adapterString)))
            {
                string adapterName = factoryAdapter.Key.Substring(0, factoryAdapter.Key.Length - adapterString.Length);
                string adapterType = factoryAdapter.Value;

                var adapterConfig = new NameValueCollection();
                foreach (var entry in properties.Where(e1 => e1.Key.StartsWith(adapterName + ".")))
                {
                    adapterConfig.Add(entry.Key.Substring(adapterName.Length + 1), entry.Value);
                }

                var adapter = (ILoggerFactoryAdapter)Activator.CreateInstance(Type.GetType(adapterType), adapterConfig);
                _loggerFactoryAdapters.Add(adapter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLoggerFactoryAdapter"/> class.
        /// </summary>
        /// <param name="factoryAdapters">The factory adapters.</param>
        public MultiLoggerFactoryAdapter(List<ILoggerFactoryAdapter> factoryAdapters)
        {
            _loggerFactoryAdapters = factoryAdapters;
        }

        protected override ILog CreateLogger(string name)
        {
            var loggers = new List<ILog>(_loggerFactoryAdapters.Count);

            foreach (var factoryAdapter in _loggerFactoryAdapters)
            {
                loggers.Add(factoryAdapter.GetLogger(name));
            }

            return new MultiLogger(loggers);
        }
    }
}