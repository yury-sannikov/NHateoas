using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Routes.RouteValueSubstitution;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    internal static class ActionsGenerator
    {
        public static MetadataPlainObjects.Actions Generate(IActionConfiguration actionConfiguration, Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            var result = new MetadataPlainObjects.Actions();

            var mappingRules = actionConfiguration.MappingRules;
            
            var routeNameSubstitution = new DefaultRouteValueSubstitution();

            result.AddRange(from mappingRule in mappingRules
                let apiDescription = mappingRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault()
                where apiDescription != null &&  apiDescription.HttpMethod != HttpMethod.Get
                let routeNames = routeRelations[apiDescription.ID]
                select new MetadataPlainObjects.Action()
                {
                    Href = routeNameSubstitution.Substitute(apiDescription.RelativePath, mappingRule, originalObject),
                    Method = apiDescription.HttpMethod.Method,
                    Title = apiDescription.Documentation,
                    ActionName = routeNames.FirstOrDefault(),
                    ActionFields = null,
                    ContentType = null
                });

            return result;
        }
    }
}
