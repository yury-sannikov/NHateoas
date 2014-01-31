using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Routes.RoutesBuilders
{
    internal class DefaultRoutesBuilder : IRoutesBuilder
    {
        public Dictionary<string, object> Build(IEnumerable<MappingRule> mappingRules, IRouteValueSubstitution routeValueSubstitution, Object data)
        {
            var result = new Dictionary<string, object>();

            foreach (var mappingRule in mappingRules)
            {
                //Get shortest route
                var ruleUrl = mappingRule.Urls.OrderBy(url => url.Url.Length).FirstOrDefault();

                if (ruleUrl == null)
                    continue;

                result[ruleUrl.RouteName] = routeValueSubstitution.Substitute(ruleUrl.Url, mappingRule, data);
            }
            return result;

        }
    }
}
