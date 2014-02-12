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
    internal class TypedMetadataPropertyStrategy : AbstractStrategy
    {
        protected readonly Type _propertyType;
        protected readonly string _propertyName;

        public TypedMetadataPropertyStrategy(Type propertyType, string propertyName)
        {
            _propertyType = propertyType;
            _propertyName = propertyName;
        }

        public override string ClassKey(Type originalType)
        {
            return string.Format("TM{0}",originalType.FullName.GetHashCode());
        }

        public override void Configure(ITypeBuilderContainer container)
        {
            var propertyVisitior = new PropertyVisitor(_propertyType, _propertyName);

            container.AddVisitor(propertyVisitior);
        }

        public override void ActivateInstance(object proxyInstance, object originalInstance, IMetadataProvider metadataProvider)
        {
            var aggregateFieldInfo = proxyInstance.GetType().GetField(PropertyVisitor.PropertyFieldName(_propertyName),
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);

            if (aggregateFieldInfo == null)
                throw new Exception("Unable to activate instance");

            aggregateFieldInfo.SetValue(proxyInstance, metadataProvider.GetMetadataByType(_propertyType, originalInstance));

        }
    }
}
