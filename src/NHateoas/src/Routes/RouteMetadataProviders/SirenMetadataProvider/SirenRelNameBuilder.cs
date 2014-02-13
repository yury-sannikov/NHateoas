using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using NHateoas.Configuration;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{

    internal class SirenRelNameBuilder : IRouteNameBuilder
    {
        private readonly Lazy<DefaultRouteNameBuilder> _defaultRouteNameBuilder = new Lazy<DefaultRouteNameBuilder>();
        [SecuritySafeCritical]
        public List<string> Build(MappingRule mappingRule, string method)
        {
            if (mappingRule.Names.Any())
                return mappingRule.Names;

            return _defaultRouteNameBuilder.Value.Build(mappingRule, method);
        }
    }
}
