using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.Visitors;
using NHateoas.Routes;

namespace NHateoas.Dynamic.Strategies
{
    internal class PayloadPropertyStrategy : TypedMetadataPropertyStrategy
    {
        public PayloadPropertyStrategy(Type propertyType, string propertyName) : base(propertyType, propertyName)
        {
        }

        public override string ClassKey(Type originalType)
        {
            return string.Format("PP{0}",originalType.FullName.GetHashCode());
        }

        public override void ActivateInstance(object proxyInstance, object originalInstance, IActionConfiguration actionConfiguration)
        {
            var aggregateFieldInfo = proxyInstance.GetType().GetField(PropertyVisitor.PropertyFieldName(_propertyName),
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);

            if (aggregateFieldInfo == null)
                throw new Exception("Unable to activate instance");

            aggregateFieldInfo.SetValue(proxyInstance, originalInstance);

        }
    }
}
