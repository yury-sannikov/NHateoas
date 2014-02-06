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
    class SirenStrategyBuilderFactory : IStrategyBuilderFactory
    {
        public ITypeBuilderStrategy Build(IActionConfiguration actionConfiguration, Type returnType)
        {
            string key = actionConfiguration.ActionMethodInfo.ToString() + returnType.ToString();

            return StrategyCache.GetCachedOrAdd(key,
                () =>
                {
                    var sirenMetadataTypes = actionConfiguration.MetadataProvider.GetRegisteredMetadataTypes().ToList();

                    var strategyBuilder = new StrategyBuilder()
                        .For(returnType)
                        .WithPayloadPropertyStrategy(returnType, "properties");

                    sirenMetadataTypes.ForEach(metadataType => strategyBuilder.WithTypedMetadataProperty(metadataType, metadataType.Name.ToLower()));

                    return strategyBuilder.Build();
                });

        }
    }
}
