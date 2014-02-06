﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;
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
                let isAction = mappingRule.Type == MappingRule.RuleType.ActionRule || (mappingRule.Type == MappingRule.RuleType.Default && apiDescription.HttpMethod != HttpMethod.Get)
                where apiDescription != null && isAction
                let routeNames = routeRelations[apiDescription.ID]
                select new MetadataPlainObjects.Action()
                {
                    Href = routeNameSubstitution.Substitute(apiDescription.RelativePath, mappingRule, originalObject),
                    Method = apiDescription.HttpMethod.Method,
                    Title = apiDescription.Documentation,
                    ActionName = routeNames.FirstOrDefault(),
                    ActionFields = ActionFieldsGenerator.Generate(mappingRule, apiDescription, originalObject),
                    ContentType = DeduceContentType(mappingRule, apiDescription, originalObject)
                });

            return result;
        }

        public static string DeduceContentType(MappingRule mappingRule, ApiDescription apiDescription, object originalObject)
        {
            if (mappingRule.ContentType != null)
                return mappingRule.ContentType.MediaType;

            if (apiDescription.HttpMethod != HttpMethod.Post)
                return null;

            if (IsOrContains(typeof(System.Web.HttpPostedFileBase), originalObject.GetType()))
                return "multipart/form-data";

            return "application/x-www-form-urlencoded";
        }

        private static bool IsOrContains(Type searchType, Type type)
        {
            if (searchType.IsAssignableFrom(type))
                return true;
            
            if (type.IsGenericType && typeof (IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                return IsOrContains(searchType.GetGenericTypeDefinition(), type);
            
            if (!type.IsClass)
                return false;
            
            return type.GetProperties().Any( p => IsOrContains(searchType, p.PropertyType));
        }
    }
}