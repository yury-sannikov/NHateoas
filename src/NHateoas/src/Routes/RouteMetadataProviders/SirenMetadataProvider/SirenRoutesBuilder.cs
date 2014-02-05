using System;
using System.Collections.Generic;
using System.Linq;
using NHateoas.Configuration;
using NHateoas.Routes.RouteMetadataProviders.SimpleMetadataProvider;
using NHateoas.Routes.RouteValueSubstitution;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    internal class SirenMetadataProvider : IMetadataProvider
    {
        private readonly IRouteNameBuilder _routeNameBuilder = new SirenRelNameBuilder();
        private readonly IRouteValueSubstitution _routeNameSubstitution = new DefaultRouteValueSubstitution();
        private readonly Dictionary<string, List<string>> _apiDescriptionToRouteNameDictionary = new Dictionary<string, List<string>>();
        private readonly IActionConfiguration _actionConfiguration;

        public SirenMetadataProvider(IActionConfiguration actionConfiguration)
        {
            _actionConfiguration = actionConfiguration;

            GenerateLinkNames();
        }
        
        private void GenerateLinkNames()
        {
            var mappingRules = _actionConfiguration.MappingRules;

            foreach (var mappingRule in mappingRules)
            {
                var apiDescription = mappingRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault();

                if (apiDescription == null)
                    continue;

                var routeName = _routeNameBuilder.Build(mappingRule, apiDescription.HttpMethod.Method);

                _apiDescriptionToRouteNameDictionary.Add(apiDescription.ID, routeName);
            }
        }

        public object GetMetadataByType(Type metadataType, params object[] values)
        {
            if (typeof (MetadataPlainObjects.Links).IsAssignableFrom(metadataType))
                return GetSirenLinksObject(values[0]);
            
            throw new NotImplementedException();
        }

        public IList<Type> GetRegisteredMetadataTypes()
        {
            return new List<Type>
            {
                typeof (MetadataPlainObjects.Links)
            };
        }

        private MetadataPlainObjects.Links GetSirenLinksObject(object originalObject)
        {
            var result = new MetadataPlainObjects.Links();

            var mappingRules = _actionConfiguration.MappingRules;

            result.AddRange(from mappingRule in mappingRules
                let apiDescription = mappingRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault()
                where apiDescription != null
                let routeNames = _apiDescriptionToRouteNameDictionary[apiDescription.ID]
                select new MetadataPlainObjects.SirenLink()
                {
                    href = _routeNameSubstitution.Substitute(apiDescription.RelativePath, mappingRule, originalObject), rel = routeNames
                });

            return result;
        }
    }
}
