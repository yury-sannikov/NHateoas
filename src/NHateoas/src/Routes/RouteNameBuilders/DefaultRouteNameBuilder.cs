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

namespace NHateoas.Routes.RouteNameBuilders
{
    internal class DefaultRouteNameBuilder : IRouteNameBuilder
    {
        public string Build(Type controller, ReflectedHttpActionDescriptor actionDescriptor, string method)
        {
            var methodName = method.ToLower();
            var name = new StringBuilder();

            var returnType = actionDescriptor.ReturnType;

            if (returnType != null)
            {
                if (typeof (HttpResponseMessage).IsAssignableFrom(returnType))
                {
                    var attributes = actionDescriptor.GetCustomAttributes<HypermediaAttribute>();
                    if (attributes != null && attributes.Count > 0)
                    {
                        returnType = attributes[0].ReturnType;
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

            var parameters = actionDescriptor.MethodInfo.GetParameters();

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
