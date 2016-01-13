using System;
using System.Collections.Generic;
using System.Linq;

namespace Noobot.Core.Logging
{
    public class AverageStat
    {
        private readonly string _unitName;
        private readonly object _lock = new object();
        private readonly List<double> _log = new List<double>();

        public AverageStat(string unitName)
        {
            _unitName = unitName;
        }

        public void Log(double value)
        {
            lock (_lock)
            {
                _log.Add(value);
            }
        }

        public override string ToString()
        {
            string value = "Nothing logged yet :-(";

            lock (_lock)
            {
                if (_log.Any())
                {
                    double total = _log.Sum(x => x);
                    value = $"{Math.Round(total/_log.Count)} {_unitName}";
                }
            }

            return value;
        }
    }
}