using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic.StrategyBuilderFactories
{
    internal static class StrategyCache
    {
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private static Dictionary<string, ITypeBuilderStrategy> _cache = new Dictionary<string, ITypeBuilderStrategy>();

        public static ITypeBuilderStrategy GetCachedOrAdd(string key, Func<ITypeBuilderStrategy> factory)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                if (_cache.ContainsKey(key))
                    return _cache[key];

                _lock.EnterWriteLock();
                try
                {
                    if (_cache.ContainsKey(key))
                        return _cache[key];

                    var strategy = factory();

                    _cache.Add(key, strategy);

                    return strategy;

                }
                finally
                {
                    _lock.ExitWriteLock();
                }

            }
            finally 
            {
                _lock.ExitUpgradeableReadLock();
            }
        }
    }
}
