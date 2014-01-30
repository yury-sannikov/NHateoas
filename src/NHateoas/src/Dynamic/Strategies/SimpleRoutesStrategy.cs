using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.Strategies;
using NHateoas.Dynamic.Visitors;

namespace NHateoas.Dynamic.Strategies
{
    internal class SimpleRoutesStrategy : AbstractStrategy
    {
        private const string ClassKeyString = "SR";

        private readonly Dictionary<string, object> _routeInformation;

        public SimpleRoutesStrategy(Dictionary<string, object> routeInformation)
        {
            _routeInformation = routeInformation;
        }

        public override string ClassKey(Type originalType)
        {
            uint routeKeysHash = _routeInformation.Keys.Aggregate((uint)0, (a, s) => a ^ (uint)s.GetHashCode());

            return string.Format("{0}_{1}_{2}", ClassKeyString, routeKeysHash, _routeInformation.Keys.Count);
        }

        public override void Configure(ITypeBuilderContainer container)
        {
            foreach (var route in _routeInformation)
            {
                container.AddVisitor(new PropertyVisitor(typeof(object), route.Key));    
            }
        }

        public override void ActivateInstance(object proxyInstance, object originalInstance,
            Dictionary<string, object> routes)
        {
            proxyInstance.GetType().GetProperties().Where(p => routes.ContainsKey(p.Name)).ToList()
                .ForEach(prop => prop.SetValue(proxyInstance, routes[prop.Name]));
        }

    }
}
