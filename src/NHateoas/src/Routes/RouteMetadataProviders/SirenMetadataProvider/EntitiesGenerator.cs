using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using NHateoas.Configuration;
using NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider.MetadataPlainObjects;
using NHateoas.Routes.RouteValueSubstitution;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    public class LinkedEntity
    {
        [JsonProperty(PropertyName = "class", NullValueHandling = NullValueHandling.Ignore)]
        public string[] ClassName;
        
        [JsonProperty(PropertyName = "href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href;

        [JsonProperty(PropertyName = "rel", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Rels;
    }

    static internal class EntitiesGenerator
    {
        public static MetadataPlainObjects.Entities Generate(IActionConfiguration actionConfiguration,
            Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            var rules = new MetadataPlainObjects.Entities();
            actionConfiguration.EntityRules.ToList()
                .ForEach(rule => rules.Add(GenerateForRule(rule, actionConfiguration, routeRelations, originalObject)));

            var result = new Entities();
            result.AddRange(rules.Where(r => r != null).ToList());
            return result;
        }

        private static object GenerateForRule(EntityRule rule, IActionConfiguration actionConfiguration,
            Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            if (rule.EntityEmbeddingRule == EntityRule.EmbeddingRule.Embedded)
                return GenerateForEmbeddedRule(rule, actionConfiguration, routeRelations, originalObject);

            return GenerateForLinkedRule(rule, actionConfiguration, routeRelations, originalObject);

        }

        private static object GenerateForLinkedRule(EntityRule rule,
            IActionConfiguration actionConfiguration,
            Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            var actionExecutedContext = ActionCallContext.Get<HttpActionExecutedContext>();
            var entityActionConfiguration = HypermediaControllerConfiguration.Instance.GetcontrollerActionConfiguration(rule.ControllerType, rule.ControllerAction, actionExecutedContext.Request.Headers.Accept);
            if (entityActionConfiguration == null)
                return null;
            
            var referencedObject = rule.GetReferencedObjectInstance(originalObject);

            if (referencedObject == null)
                return null;
            var selfRule = entityActionConfiguration.MappingRules.FirstOrDefault(r => r.Names.Contains("self"));

            if (selfRule == null)
                throw new Exception(string.Format("Unable to generate link to entity object from controller {0} action {1}. Can't find self rel.", actionConfiguration.ControllerType.FullName, actionConfiguration.ActionMethodInfo));

            var selfApi = selfRule.ApiDescriptions.OrderBy(d => d.RelativePath.Length).FirstOrDefault();

            if (selfApi == null)
                throw new Exception(string.Format("Unable to generate link to entity object from controller {0} action {1}. Can't find self API.", actionConfiguration.ControllerType.FullName, actionConfiguration.ActionMethodInfo));
            
            var routeNameSubstitution = new DefaultRouteValueSubstitution();

            var absolutePath =
                LinkHelper.MakeAbsolutePath(routeNameSubstitution.Substitute(selfApi.RelativePath, selfRule,
                    referencedObject));
            
            return new LinkedEntity()
            {
                Rels = rule.Rel,
                ClassName = entityActionConfiguration.Class,
                Href = absolutePath
            };
        }


        private static object GenerateForEmbeddedRule(EntityRule rule, IActionConfiguration actionConfiguration,
            Dictionary<string, List<string>> routeRelations, object originalObject)
        {
            var actionExecutedContext = ActionCallContext.Get<HttpActionExecutedContext>();
            var entityActionConfiguration = HypermediaControllerConfiguration.Instance.GetcontrollerActionConfiguration(rule.ControllerType, rule.ControllerAction, actionExecutedContext.Request.Headers.Accept);
            if (entityActionConfiguration == null)
                return null;

            var referencedObject = rule.GetReferencedObjectInstance(originalObject);
            
            if (referencedObject == null)
                return null;

            var transformed = ActionResponseTransformer.TransformPayload(entityActionConfiguration, referencedObject);

            if (transformed == null)
                return null;

            if (transformed is IEnumerable)
                throw new Exception(string.Format("Unable to generate enumerable entity object from controller {0} action {1}. Consider using outer object.", actionConfiguration.ControllerType.FullName, actionConfiguration.ActionMethodInfo));

            AssignRelName(transformed, rule.Rel);

            return transformed;
        }

        private static void AssignRelName(object payload, string[] rel)
        {
            if (payload == null)
                return;

            var relProp = payload.GetType().GetProperty("rel");
            
            if (relProp == null)
                return;
            
            relProp.SetValue(payload, rel);
        }

    }
}
