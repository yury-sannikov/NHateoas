using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.Visitors;

namespace NHateoas.Dynamic.Strategies
{
    internal class SimplePropertyStrategy : AbstractStrategy
    {
        private readonly Type _propType;
        private readonly string _propName;
        private readonly Func<CustomAttributeBuilder>[] _attributeFactories;

        public SimplePropertyStrategy(Type propType, string propName,
            Func<CustomAttributeBuilder>[] attributeFactories)
        {
            _propType = propType;
            _propName = propName;
            _attributeFactories = attributeFactories;
        }

        public override string ClassKey(Type originalType)
        {
            return string.Format("SPS{0}_{1}_{2}_{3}", originalType.FullName.GetHashCode(), _propType.FullName.GetHashCode(), _propName, _attributeFactories.Length);
        }

        public override void Configure(ITypeBuilderContainer container)
        {
            var propVisitor = new PropertyVisitor(_propType, _propName);

            if (_attributeFactories != null)
                _attributeFactories.ToList().ForEach(propVisitor.AddCustomAttributeFactory);

            container.AddVisitor(propVisitor);
        }
    }
}
