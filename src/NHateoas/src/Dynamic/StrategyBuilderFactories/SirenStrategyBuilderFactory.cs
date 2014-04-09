using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

                    Func<CustomAttributeBuilder> jsonPropAttrNullValueHandling = () =>
                    {
                        var type = typeof (JsonPropertyAttribute);

                        var ctor = type.GetConstructor(Type.EmptyTypes);

                        return new CustomAttributeBuilder(ctor,
                            new object[] {},
                            new[] {type.GetProperty("NullValueHandling")}, new object[] {NullValueHandling.Ignore});
                    };

                    var strategyBuilder = new StrategyBuilder()
                        .For(returnType)
                        .WithPayloadPropertyStrategy(returnType, "properties")
                        .WithSimpleAttributedPropertyStrategy(typeof(string[]), "class", new [] { jsonPropAttrNullValueHandling })
                        .WithSimpleAttributedPropertyStrategy(typeof(string), "href", new[] { jsonPropAttrNullValueHandling })
                        .WithSimpleAttributedPropertyStrategy(typeof(string[]), "rel", new[] { jsonPropAttrNullValueHandling });

                    sirenMetadataTypes.ForEach(metadataType => strategyBuilder.WithTypedMetadataProperty(metadataType, metadataType.Name.ToLower()));

                    return strategyBuilder.Build();
                });

        }
    }
}
