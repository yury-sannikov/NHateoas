using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.Visitors;
using NHateoas.Routes;

namespace NHateoas.Dynamic.Strategies
{
    internal class SimplePropertiesAggregatedStrategy : AbstractStrategy
    {
        private const string SimplePropertiesAggregatedStrategyName = "SP";

        private const string AggregateFieldName = "__Agg2AC613AF7A2C4A2D9417321E7D1A128B";

        private readonly HashSet<Type> _complexTypes;

        private readonly Type _originalType;
        
        public SimplePropertiesAggregatedStrategy(Type originalType, HashSet<Type> complexTypes)
        {
            _originalType = originalType;
            _complexTypes = complexTypes;
        }

        public override string ClassKey(Type originalType)
        {
            return SimplePropertiesAggregatedStrategyName;
        }

        public override void Configure(ITypeBuilderContainer container)
        {

            var aggregateVisitor = new AggregateTypeBuilder(_originalType, AggregateFieldName);
            
            container.AddVisitor(aggregateVisitor);

            foreach (var property in _originalType.GetProperties())
            {
                if (_complexTypes.Contains(property.PropertyType))
                    continue;
                
                aggregateVisitor.AddVisitor(
                        new AggregatedPropertyVisitor(property)
                    );
            }
        }

        public override void ActivateInstance(object proxyInstance, object originalInstance, IRoutesBuilder routesBuilder)
        {
            var aggregateFieldInfo = proxyInstance.GetType().GetField(AggregateFieldName,
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);

            if (aggregateFieldInfo == null)
                throw new Exception("Unable to activate instance");

            aggregateFieldInfo.SetValue(proxyInstance, originalInstance);

        }
    }
}
