using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.Strategies;

namespace NHateoas.Dynamic.StrategyBuilderFactories
{
    class DefaultStrategyBuilderFactory : IStrategyBuilderFactory
    {
        public ITypeBuilderStrategy Build(IActionConfiguration actionConfiguration, Type returnType)
        {
            string key = actionConfiguration.ActionMethodInfo.ToString() + returnType.ToString();

            return StrategyCache.GetCachedOrAdd(key,
                () =>
                {
                    var rels = actionConfiguration.RoutesBuilder.GetRels();

                    IList<string> topRels = rels.Values.ToList().ConvertAll(c => c.FirstOrDefault());

                    var strategyBuilder = new StrategyBuilder()
                        .For(returnType)
                        .WithSimpleProperties()
                        .WithPlainRouteInformation(topRels);

                    return strategyBuilder.Build();
                });

        }
    }
}
