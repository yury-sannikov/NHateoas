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
        private static ConcurrentDictionary<string, Lazy<ITypeBuilderStrategy>> _cache = new ConcurrentDictionary<string, Lazy<ITypeBuilderStrategy>>();

        public static ITypeBuilderStrategy GetCachedOrAdd(string key, Func<ITypeBuilderStrategy> factory)
        {
            return _cache.GetOrAdd(key, new Lazy<ITypeBuilderStrategy>(factory, LazyThreadSafetyMode.ExecutionAndPublication)).Value;
        }

        internal static void Teardown()
        {
            _cache.Clear();
        }
    }
}
