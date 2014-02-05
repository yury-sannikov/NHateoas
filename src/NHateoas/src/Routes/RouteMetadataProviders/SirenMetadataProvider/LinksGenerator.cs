using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Routes.RouteValueSubstitution;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    internal static class LinksGenerator
    {
        public static MetadataPlainObjects.Links Generate(IActionConfiguration actionConfiguration, Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            var result = new MetadataPlainObjects.Links();

            var mappingRules = actionConfiguration.MappingRules;
            
            var routeNameSubstitution = new DefaultRouteValueSubstitution();

            result.AddRange(from mappingRule in mappingRules
                let apiDescription = mappingRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault()
                where apiDescription != null
                            let routeNames = routeRelations[apiDescription.ID]
                select new MetadataPlainObjects.SirenLink()
                {
                    Href = routeNameSubstitution.Substitute(apiDescription.RelativePath, mappingRule, originalObject), RelList = routeNames
                });

            return result;
        }
    }
}
