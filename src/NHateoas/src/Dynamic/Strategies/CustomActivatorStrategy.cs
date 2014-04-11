using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic.Strategies
{
    internal class CustomActivatorStrategy : AbstractStrategy
    {
        private readonly Action<object, object, IActionConfiguration> _customActivator;

        public CustomActivatorStrategy(Action<object, object, IActionConfiguration> customActivator)
        {
            _customActivator = customActivator;
        }
        
        public override string ClassKey(Type originalType)
        {
            return string.Format("CAS_{0}_{1}", originalType.FullName.GetHashCode(), this.GetHashCode());
        }

        public override void Configure(ITypeBuilderContainer container)
        {
        }

        public override void ActivateInstance(object proxyInstance, object originalInstance, IActionConfiguration actionConfiguration)
        {
            if (_customActivator != null)
                _customActivator(proxyInstance, originalInstance, actionConfiguration);
        }
    }
}
