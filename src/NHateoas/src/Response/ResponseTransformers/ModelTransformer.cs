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
        public object Transform(IActionConfiguration actionConfiguration, object payload)
        {
            var strategyFactory = actionConfiguration.StrategyBuilderFactory;

            var strategy = strategyFactory.Build(actionConfiguration, payload.GetType());

            var typeBuilder = new TypeBuilder(payload.GetType(), strategy);

            var proxyType = typeBuilder.BuildType();

            var newinstance = Activator.CreateInstance(proxyType);

            strategy.ActivateInstance(newinstance, payload, actionConfiguration);

            return newinstance;
        }

        public bool CanTransform(object data)
        {
            var type = data.GetType();
            return type.IsClass && !type.IsGenericType && !type.IsAbstract && type.IsPublic && type.IsVisible;
        }
    }
}
