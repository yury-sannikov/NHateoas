using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Routes
{
    internal interface IRoutesBuilder
    {
        Dictionary<string, IList<string>> GetRels();

        Dictionary<string, object> Build(Object data);
    }
}
