using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Routes.RoutesBuilders
{
    internal class DefaultRoutesBuilder : IRoutesBuilder
    {
        public Dictionary<string, object> Build(IEnumerable<MappingRule> mappingRules)
        {
            var result = new Dictionary<string, object>();

            foreach (var mappingRule in mappingRules)
            {
                //Get shortest route
                var ruleUrl = mappingRule.Urls.OrderBy(url => url.Url.Length).FirstOrDefault();

                if (ruleUrl == null)
                    continue;

                result[ruleUrl.RouteName] = ruleUrl.Url;
            }
            return result;

        }
    }
}
