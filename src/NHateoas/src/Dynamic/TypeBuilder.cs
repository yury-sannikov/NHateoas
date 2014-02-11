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

        private static readonly ConcurrentDictionary<string, Lazy<Type>> _cache = new ConcurrentDictionary<string, Lazy<Type>>();

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

        private Type TypeFactory()
        {
            var moduleBuilder = ModuleBuilderFactory.Instance;

            var className = string.Format("{0}.{1}", _typeBuilderStrategy.ClassKey(_originalType), _originalType.Name);

            var typeBuilder = moduleBuilder.DefineType(className, _typeAttributes, _parentType);

            var provider = new TypeBuilderProvider(typeBuilder);

            _typeBuilderVisitors.ForEach(v => v.Visit(provider));

            return typeBuilder.CreateType();

        }

        public Type BuildType()
        {
            var key = _typeBuilderStrategy.ClassKey(_originalType);

            return _cache.GetOrAdd(key, new Lazy<Type>(TypeFactory, LazyThreadSafetyMode.ExecutionAndPublication)).Value;

        }

        public ITypeBuilderContainer AddVisitor(ITypeBuilderVisitor visitor)
        {
            _typeBuilderVisitors.Add(visitor);
            return this;
        }

    }
}
