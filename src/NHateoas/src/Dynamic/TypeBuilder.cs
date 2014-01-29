using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic
{

    internal class TypeBuilder : ITypeBuilderContainer
    {
        private readonly Type _originalType;
        private readonly Type _parentType = typeof(Object);
        private readonly ITypeBuilderStrategy _typeBuilderStrategy;

        private const TypeAttributes _typeAttributes = TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit;

        private readonly List<ITypeBuilderVisitor> _typeBuilderVisitors = new List<ITypeBuilderVisitor>();

        private static readonly ConcurrentDictionary<string, Type> _cache = new ConcurrentDictionary<string, Type>();

        private static readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        class TypeBuilderProvider : ITypeBuilderProvider
        {
            private readonly System.Reflection.Emit.TypeBuilder _typeBuilder = null;

            public TypeBuilderProvider(System.Reflection.Emit.TypeBuilder typeBuilder)
            {
                _typeBuilder = typeBuilder;
            }

            public System.Reflection.Emit.TypeBuilder GetTypeBuilder()
            {
                return _typeBuilder;
            }
        }

        public TypeBuilder(Type originalType, ITypeBuilderStrategy typeBuilderStrategy)
        {
            _originalType = originalType;
            _typeBuilderStrategy = typeBuilderStrategy;

            _typeBuilderStrategy.Configure(this);
        }

        public Type BuildType()
        {
            _readerWriterLockSlim.EnterUpgradeableReadLock();

            try
            {
                var cachedType = GetTypeFromCache();

                if (cachedType != null)
                    return cachedType;

                _readerWriterLockSlim.EnterWriteLock();

                try
                {
                    // Double lock pattern. Other thread might already created the same type while waiting on entering lock
                    cachedType = GetTypeFromCache();

                    if (cachedType != null)
                        return cachedType;

                    var moduleBuilder = ModuleBuilderFactory.Instance;

                    var className = string.Format("{0}.{1}", _typeBuilderStrategy.ClassKey(_originalType), _originalType.Name);

                    var typeBuilder = moduleBuilder.DefineType(className, _typeAttributes, _parentType);

                    var provider = new TypeBuilderProvider(typeBuilder);

                    _typeBuilderVisitors.ForEach(v => v.Visit(provider));

                    var resultType = typeBuilder.CreateType();

                    StoreTypeToCache(resultType);

                    return resultType;
                }
                finally
                {
                    _readerWriterLockSlim.ExitWriteLock();
                }
            }
            finally 
            {
                _readerWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        private void StoreTypeToCache(Type resultType)
        {
            var key = _typeBuilderStrategy.ClassKey(_originalType);

            _cache.TryAdd(key, resultType);
        }

        private Type GetTypeFromCache()
        {
            var key = _typeBuilderStrategy.ClassKey(_originalType);

            if (!_cache.ContainsKey(key))
                return null;
            
            Type resultType;

            _cache.TryGetValue(key, out resultType);

            return resultType;
        }

        public ITypeBuilderContainer AddVisitor(ITypeBuilderVisitor visitor)
        {
            _typeBuilderVisitors.Add(visitor);
            return this;
        }

    }
}
