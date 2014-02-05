using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Routes.RouteValueSubstitution;

namespace NHateoas.Routes.RouteMetadataProviders.SimpleMetadataProvider
{
    internal class SimpleMetadataProvider : IMetadataProvider
    {
        private IRouteNameBuilder _routeNameBuilder = null;
        private IRouteValueSubstitution _routeNameSubstitution = null;
        private readonly Dictionary<string, string> _apiDescriptionToRouteNameDictionary = new Dictionary<string, string>();
        private readonly IActionConfiguration _actionConfiguration;

        public SimpleMetadataProvider(IActionConfiguration actionConfiguration)
        {
            _actionConfiguration = actionConfiguration;

            GenerateLinkNames();
        }

        public object GetMetadataByType(Type metadataType, params object[] values)
        {
            if (typeof (Dictionary<string, object>).IsAssignableFrom(metadataType))
            {
                return Build(values[0]);
            }
            
            if (typeof (Dictionary<string, IList<string>>).IsAssignableFrom(metadataType))
            {
                return GetRels();
            }

            throw new NotImplementedException();
        }

        public IList<Type> GetRegisteredMetadataTypes()
        {
            return new List<Type> { typeof(Dictionary<string, object>), typeof(Dictionary<string, IList<string>>) };
        }

        private Dictionary<string, IList<string>> GetRels()
        {
            return _apiDescriptionToRouteNameDictionary.ToList().ToDictionary(ks => ks.Key, vs => (IList<string>)new List<string> { vs.Value });
        }

        private void GenerateLinkNames()
        {
            var mappingRules = _actionConfiguration.MappingRules;

            foreach (var mappingRule in mappingRules)
            {
                var apiDescription = mappingRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault();

                if (apiDescription == null)
                    continue;

                var routeName = RouteNameBuilder.Build(mappingRule, apiDescription.HttpMethod.Method);

                _apiDescriptionToRouteNameDictionary.Add(apiDescription.ID, routeName.FirstOrDefault());
            }
        }

        private Dictionary<string, object> Build(Object data)
        {
            var result = new Dictionary<string, object>();
            
            var mappingRules = _actionConfiguration.MappingRules;

            foreach (var mappingRule in mappingRules)
            {
                //Get shortest route
                var apiDescription = mappingRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault();

                if (apiDescription == null)
                    continue;

                var routeName = _apiDescriptionToRouteNameDictionary[apiDescription.ID];
                
                result[routeName] = RouteValueSubstitution.Substitute(apiDescription.RelativePath, mappingRule, data);
            }
            return result;

        }
        
        protected IRouteNameBuilder RouteNameBuilder
        {
            get { return _routeNameBuilder ?? (_routeNameBuilder = new DefaultRouteNameBuilder()); }
            set { _routeNameBuilder = value; }
        }
        
        protected IRouteValueSubstitution RouteValueSubstitution
        {
            get { return _routeNameSubstitution ?? (_routeNameSubstitution = new DefaultRouteValueSubstitution()); }
            set { _routeNameSubstitution = value; }
        }

    }
}
