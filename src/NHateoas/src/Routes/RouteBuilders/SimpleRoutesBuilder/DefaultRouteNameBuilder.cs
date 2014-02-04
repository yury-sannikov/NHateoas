using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using NHateoas.Attributes;
using NHateoas.Configuration;

namespace NHateoas.Routes.RouteBuilders.SimpleRoutesBuilder
{
    internal class DefaultRouteNameBuilder : IRouteNameBuilder
    {
        public string Build(Type controller, MethodInfo actionMethodInfo, string method)
        {
            var methodName = method.ToLower();
            var name = new StringBuilder();

            var returnType = actionMethodInfo.ReturnType;

            if (returnType != typeof(void))
            {
                if (typeof (HttpResponseMessage).IsAssignableFrom(returnType))
                {
                    var attributes = actionMethodInfo.GetCustomAttributes<HypermediaAttribute>().ToList();
                    if (attributes.Any())
                    {
                        returnType = attributes.First().ReturnType;
                    }
                }
                
                if (returnType.IsGenericType && typeof (IEnumerable<>).IsAssignableFrom(returnType.GetGenericTypeDefinition()))
                {
                    returnType = returnType.GetGenericArguments()[0];

                    methodName = "query";
                }
            }

            name.Append(methodName);

            if (returnType != null)
                name.AppendFormat("_{0}", returnType.Name.ToLower());

            var parameters = actionMethodInfo.GetParameters();

            if (parameters.Any())
                name.AppendFormat("_by");

            foreach (var parameterInfo in parameters)
            {
                if (parameterInfo.GetType() == returnType)
                    continue;
                
                name.AppendFormat("_{0}", parameterInfo.Name.ToLower());
            }

            return name.ToString();
        }
    }
}
