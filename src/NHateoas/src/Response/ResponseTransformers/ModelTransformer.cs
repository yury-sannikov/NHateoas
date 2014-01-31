using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Dynamic;
using NHateoas.Dynamic.Strategies;

namespace NHateoas.Response.ResponseTransformers
{
    class ModelTransformer : IResponseTransformer
    {
        public object Transform(ActionConfiguration actionConfiguration, object payload)
        {
            Dictionary<string, object> routes = actionConfiguration.RoutesBuilder.Build(actionConfiguration.MappingRules, actionConfiguration.RouteValueSubstitution, payload);

            var strategyBuilder = new StrategyBuilder()
                .For(payload.GetType())
                .WithSimpleProperties()
                .WithRouteInformation(routes);

            var strategy = strategyBuilder.Build();

            var typeBuilder = new TypeBuilder(payload.GetType(), strategy);

            var proxyType = typeBuilder.BuildType();

            var newinstance = Activator.CreateInstance(proxyType);

            strategy.ActivateInstance(newinstance, payload, routes);
            return newinstance;
        }

        public bool CanTransform(object data)
        {
            var type = data.GetType();
            return type.IsClass && !type.IsGenericType;
        }
    }
}
