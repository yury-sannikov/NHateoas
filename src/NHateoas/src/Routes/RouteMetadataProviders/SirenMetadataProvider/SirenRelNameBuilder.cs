using System;
using System.Collections.Generic;
using System.Linq;
using NHateoas.Configuration;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    internal class SirenRelNameBuilder : IRouteNameBuilder
    {
        private readonly Lazy<DefaultRouteNameBuilder> _defaultRouteNameBuilder = new Lazy<DefaultRouteNameBuilder>();

        public List<string> Build(MappingRule mappingRule, string method)
        {
            if (mappingRule.Rels.Any())
                return mappingRule.Rels;

            return _defaultRouteNameBuilder.Value.Build(mappingRule, method);
        }
    }
}
