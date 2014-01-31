using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using NHateoas.Configuration;

namespace NHateoas.Routes
{
    /// <summary>
    /// Match controller's actions with Web API routing information
    /// </summary>
    internal static class RoutesBuilder
    {
        public static void Build(Type controllerType, ActionConfiguration actionConfiguration, IApiExplorer apiExplorer)
        {
            var nameBuilder = actionConfiguration.RouteNameBuilder;

            foreach (var description in apiExplorer.ApiDescriptions)
            {
                var actionDescriptor = description.ActionDescriptor as ReflectedHttpActionDescriptor;

                if (actionDescriptor == null)
                    continue;

                var rule = actionConfiguration.MappingRules.FirstOrDefault(r => r.MethodExpression.Method == actionDescriptor.MethodInfo);

                if (rule == null)
                    continue;
                
                rule.AddUrl(new MappingRuleUrl()
                {
                    RouteName = nameBuilder.Build(controllerType, actionDescriptor, description.HttpMethod.Method),
                    Method = description.HttpMethod.Method,
                    Url = description.RelativePath,
                    Documentation = description.Documentation
                });

            }
            
        }
    }
}
