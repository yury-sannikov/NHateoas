using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider.MetadataPlainObjects;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    static internal class EntitiesGenerator
    {
        public static MetadataPlainObjects.Entities Generate(IActionConfiguration actionConfiguration,
            Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            var rules = new MetadataPlainObjects.Entities();
            actionConfiguration.EntityRules.ToList()
                .ForEach(rule => rules.AddRange(GenerateForRule(rule, actionConfiguration, routeRelations, originalObject).Where(r => r != null)));
            return rules;
        }

        private static IEnumerable<object> GenerateForRule(EntityRule rule, IActionConfiguration actionConfiguration,
            Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            var entityActionConfiguration = HypermediaControllerConfiguration.Instance.GetcontrollerActionConfiguration(rule.ControllerType, rule.ControllerAction);
            if (entityActionConfiguration == null)
                return null;

            var referencedObject = rule.GetReferencedObjectInstance(originalObject);
            
            if (referencedObject == null)
                return null;

            var transformed = ActionResponseTransformer.TransformPayload(entityActionConfiguration, referencedObject);

            if (transformed == null)
                return null;

            var asEnumerable = transformed as IEnumerable<object>;
            
            return asEnumerable ?? new List<object>() {transformed};
        }

    }
}
