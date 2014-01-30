using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace NHateoas.Configuration
{
    internal interface IRouteNameBuilder
    {
        string Build(Type controller, ReflectedHttpActionDescriptor actionDescriptor, string method);
    }
}
