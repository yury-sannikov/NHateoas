using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Routes.RouteValueSubstitution
{
    internal class DefaultRouteValueSubstitution : IRouteValueSubstitution
    {
        public string Substitute(string templateUrl, MappingRule mapping, Object data)
        {
            var methodParameters = mapping.MethodExpression.Method.GetParameters();

            var result = new StringBuilder(templateUrl);

            foreach (var methodParameter in methodParameters)
            {
                var parameterTemplateName = string.Format("{{{0}}}", methodParameter.Name);

                if (!templateUrl.Contains(parameterTemplateName))
                    continue;

                if (!mapping.ParameterDelegates.ContainsKey(methodParameter.Name))
                {
                    Debug.Write(string.Format("Unable to obtain delegate for parameter {0}, URL: {1}", methodParameter.Name, templateUrl));
                    continue;
                }

                var paramDelegate = mapping.ParameterDelegates[methodParameter.Name];

                var paramResult = paramDelegate.DynamicInvoke(data);

                if (paramResult == null)
                {
                    Debug.Write(string.Format("Unable to get result for parameter {0}, URL: {1}", methodParameter.Name, templateUrl));
                    continue;
                }

                result.Replace(parameterTemplateName, paramResult.ToString());
            }

            return result.ToString();
        }
    }
}
