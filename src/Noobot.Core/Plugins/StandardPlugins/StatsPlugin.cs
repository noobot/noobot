using System;
using System.Collections.Generic;
using System.Linq;
using Noobot.Core.Logging;

namespace Noobot.Core.Plugins.StandardPlugins
{
    public class StatsPlugin : IPlugin
    {
        private readonly ILog _log;
        private readonly Dictionary<string, object> _stats = new Dictionary<string, object>();
        private readonly object _lock = new object();

        public StatsPlugin(ILog log)
        {
            _log = log;
        }

        /// <summary>
        /// Sets a key to the given value
        /// </summary>
        public void RecordStat(string key, object value)
        {
            lock (_lock)
            {
                _stats[key] = value;
            }
        }

        /// <summary>
        /// Assumes current stat is a number and increments it by 1. Defaults to 1 if key doesn't exist.
        /// </summary>
        public void IncrementState(string key)
        {
            lock (_lock)
            {
                int? value2Store = null;
                if (_stats.ContainsKey(key))
                {
                    value2Store = _stats[key] as int?;

                    if (value2Store.HasValue)
                    {
                        value2Store += 1;
                    }
                }

                _stats[key] = value2Store.HasValue ? value2Store : 1;
            }
        }

        public string[] GetStats()
        {
            var list = new List<string>(_stats.Count);

            lock (_lock)
            {
                list.AddRange(_stats.Keys.Select(key => $"{key}: {_stats[key]}"));
            }

            return list.ToArray();
        }

        public T GetStat<T>(string key)
        {
            T result = default(T);

            lock (_lock)
            {
                if (_stats.ContainsKey(key))
                {
                    result = (T)_stats[key];
                }
            }

            return result;
        }

        public void Start()
        { }

        public void Stop()
        {
            _log.Log("End stats:");
            _log.Log(string.Join(Environment.NewLine + "   ", GetStats()));
        }
    }
}