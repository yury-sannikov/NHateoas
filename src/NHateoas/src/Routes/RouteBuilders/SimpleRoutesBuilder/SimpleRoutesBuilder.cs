using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Routes.RouteValueSubstitution;

namespace NHateoas.Routes.RouteBuilders.SimpleRoutesBuilder
{
    internal class SimpleRoutesBuilder : IRoutesBuilder
    {
        private IRouteNameBuilder _routeNameBuilder = null;
        private IRouteValueSubstitution _routeNameSubstitution = null;
        private readonly Dictionary<string, string> _apiDescriptionToRouteNameDictionary = new Dictionary<string, string>();
        private readonly ActionConfiguration _actionConfiguration;

        public SimpleRoutesBuilder(ActionConfiguration actionConfiguration)
        {
            _actionConfiguration = actionConfiguration;

            GenerateLinkNames();
        }

        public Dictionary<string, IList<string>> GetRels()
        {
            return _apiDescriptionToRouteNameDictionary.ToList().ToDictionary(ks => ks.Key, vs => (IList<string>)new List<string>{vs.Value});
        }

        private void GenerateLinkNames()
        {
            var mappingRules = _actionConfiguration.MappingRules;

            foreach (var mappingRule in mappingRules)
            {
                var apiDescription = mappingRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault();

                if (apiDescription == null)
                    continue;

                var ruleMethod = mappingRule.MethodExpression.Method;

                var routeName = RouteNameBuilder.Build(ruleMethod.DeclaringType, ruleMethod, apiDescription.HttpMethod.Method);

                _apiDescriptionToRouteNameDictionary.Add(apiDescription.ID, routeName);
            }
        }

        public Dictionary<string, object> Build(Object data)
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
