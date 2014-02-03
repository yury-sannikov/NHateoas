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
        public static void Build(IHypermediaControllerConfiguration controllerConfiguration, Type controllerType, MethodInfo executingActionMethodInfo, IApiExplorer apiExplorer)
        {
            var actionConfiguration = controllerConfiguration.GetcontrollerActionConfiguration(controllerType,
                executingActionMethodInfo);

            if (actionConfiguration == null || actionConfiguration.RulesHasBeenBuilt)
               return;

            BuildLocalRules(controllerType, apiExplorer, actionConfiguration);

            BuildForeignRules(controllerConfiguration, controllerType, apiExplorer, actionConfiguration);
        }

        private static void BuildLocalRules(Type controllerType, IApiExplorer apiExplorer, ActionConfiguration actionConfiguration)
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

        private static void BuildForeignRules(IHypermediaControllerConfiguration controllerConfiguration, Type controllerType, IApiExplorer apiExplorer,
            ActionConfiguration actionConfiguration)
        {
            var referencedRules = actionConfiguration.MappingRules.Where(r => r.MethodExpression.Method.DeclaringType != controllerType).ToList();

            if (!referencedRules.Any())
                return;

            foreach (var referencedRule in referencedRules)
            {
                var foreignControllerType = referencedRule.MethodExpression.Method.DeclaringType;
                var foreignMethodInfo = referencedRule.MethodExpression.Method;
                
                //Build foreign controller rules
                Build(controllerConfiguration, foreignControllerType, foreignMethodInfo, apiExplorer);

                var foreignActionConfiguration = controllerConfiguration.GetcontrollerActionConfiguration(foreignControllerType, foreignMethodInfo);
                
                if (foreignActionConfiguration == null)
                    continue;

                var foreignRule = foreignActionConfiguration.MappingRules.FirstOrDefault(r => r.MethodExpression.Method == foreignMethodInfo);

                if (foreignRule == null)
                    continue;

                foreignRule.Urls.ToList().ForEach(url => referencedRule.AddUrl(url));
            }
        }
    }
}
