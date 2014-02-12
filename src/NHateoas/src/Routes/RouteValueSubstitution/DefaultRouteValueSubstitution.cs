using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;
using System.Web;

namespace NHateoas.Routes.RouteValueSubstitution
{
    internal class DefaultRouteValueSubstitution : IRouteValueSubstitution
    {
        public string Substitute(string templateUrl, MappingRule mapping, Object data)
        {
            var methodParameters = mapping.MethodExpression.Method.GetParameters();

            var expressionArguments = mapping.MethodExpression.Arguments.GetEnumerator();

            var result = new StringBuilder(templateUrl);

            foreach (var methodParameter in methodParameters)
            {
                expressionArguments.MoveNext();

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

                var stringResult = paramResult.ToString();

                var expressionArgunemt = (expressionArguments.Current as MethodCallExpression);
                
                if (expressionArgunemt != null && expressionArgunemt.Method.DeclaringType == typeof (QueryParameter))
                {
                    result.Replace(parameterTemplateName, stringResult);    
                }
                else
                {
                    result.Replace(parameterTemplateName, HttpUtility.UrlEncode(stringResult));
                }

                
            }

            return result.ToString();
        }
    }
}
