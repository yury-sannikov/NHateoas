using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using NHateoas.Configuration;

namespace NHateoas.Routes.RouteNameBuilders
{
    internal class DefaultRouteNameBuilder : IRouteNameBuilder
    {
        public string Build(Type controller, ReflectedHttpActionDescriptor actionDescriptor, string method)
        {
            var name = new StringBuilder(method.ToLower());


            var returnType = actionDescriptor.ReturnType;

            if (returnType != null)
                name.AppendFormat("_{0}_by", returnType.Name.ToLower());    
            
            foreach (var parameterInfo in actionDescriptor.MethodInfo.GetParameters())
            {
                name.AppendFormat("_{0}", parameterInfo.Name.ToLower());
            }

            return name.ToString();
        }
    }
}
