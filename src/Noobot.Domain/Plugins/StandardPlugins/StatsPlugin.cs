using System;
using System.Collections.Generic;
using System.Linq;

namespace Noobot.Domain.Plugins.StandardPlugins
{
    public class StatsPlugin : IPlugin
    {
        private readonly Dictionary<string, object> _stats = new Dictionary<string, object>();
        private readonly object _lock = new object();

        public void RecordStat(string key, int value)
        {
            RecordStat(key, value.ToString());
        }

        public void RecordStat(string key, string value)
        {
            lock (_lock)
            {
                _stats[key] = value;
            }
        }

        public void IncrementState(string key)
        {
            lock (_lock)
            {
                int? value2Store = null;
                if (_stats.ContainsKey(key))
                {
                    value2Store = _stats[key] as int?;
                    value2Store += 1;
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

        public void Start()
        { }

        public void Stop()
        {
            Console.WriteLine("End stats:");
            Console.WriteLine(string.Join(Environment.NewLine + "   ", GetStats()));
        }
    }
}