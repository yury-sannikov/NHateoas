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
using NHateoas.Routes;

namespace NHateoas.Dynamic.Strategies
{
    internal class SimpleRoutesStrategy : AbstractStrategy
    {
        private const string ClassKeyString = "SR";

        private readonly IList<string> _routeInformation;

        public SimpleRoutesStrategy(IList<string> routeInformation)
        {
            _routeInformation = routeInformation;
        }

        public override string ClassKey(Type originalType)
        {
            uint routeKeysHash = _routeInformation.Aggregate((uint)0, (a, s) => a ^ (uint)s.GetHashCode());

            return string.Format("{0}_{1}_{2}", ClassKeyString, routeKeysHash, _routeInformation.Count);
        }

        public override void Configure(ITypeBuilderContainer container)
        {
            foreach (var route in _routeInformation)
            {
                container.AddVisitor(new PropertyVisitor(typeof(object), route));    
            }
        }

        public override void ActivateInstance(object proxyInstance, object originalInstance,
            IRoutesBuilder routesBuilder)
        {
            Dictionary<string, object> routes = routesBuilder.Build(originalInstance);

            proxyInstance.GetType().GetProperties().Where(p => routes.ContainsKey(p.Name)).ToList()
                .ForEach(prop => prop.SetValue(proxyInstance, routes[prop.Name]));
        }

    }
}
