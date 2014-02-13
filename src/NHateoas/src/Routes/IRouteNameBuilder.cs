using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using NHateoas.Configuration;

namespace NHateoas.Routes
{
    internal interface IRouteNameBuilder
    {
        [SecuritySafeCritical]
        List<string> Build(MappingRule mappingRule, string method);
    }
}
