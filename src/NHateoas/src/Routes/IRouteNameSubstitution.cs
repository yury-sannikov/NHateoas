using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Routes
{
    internal interface IRouteValueSubstitution
    {
        string Substitute(string templateUrl, MappingRule mapping, Object data);
    }
}
