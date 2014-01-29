using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic.Strategies
{
    internal class StrategyBuilder : AbstractStrategy
    {
        private readonly List<ITypeBuilderStrategy> _compositeStrategiesList = new List<ITypeBuilderStrategy>();
        private readonly HashSet<Type> _complexTypes = new HashSet<Type>();
        private Type _objectType;

        #region AbstractStrategy
        public override string ClassKey(Type originalType)
        {
            return _compositeStrategiesList.Aggregate("_"  + originalType.FullName, (s, strategy) => string.Format("{0}{1}_", s, strategy.ClassKey(originalType)));
        }

        public override void Configure(ITypeBuilderContainer container)
        {
            _compositeStrategiesList.ForEach(strategy => strategy.Configure(container));
        }
        #endregion

        #region IInstanceActivator
        public override void ActivateInstance(object proxyInstance, object originalInstance, Dictionary<string, string> routes)
        {
            _compositeStrategiesList.ForEach(a => a.ActivateInstance(proxyInstance, originalInstance, routes));
        }
        #endregion

        #region Fluent
        public StrategyBuilder For(Type objectType)
        {
            _objectType = objectType;
            return this;
        }

        public StrategyBuilder WithSimpleProperties()
        {
            _compositeStrategiesList.Add(
                new SimplePropertiesAggregatedStrategy(_objectType, _complexTypes)
                );
            return this;
        }

        public ITypeBuilderStrategy Build()
        {
            return this;
        }
        #endregion
    }
}
