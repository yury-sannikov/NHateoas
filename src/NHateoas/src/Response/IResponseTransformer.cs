using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Response
{
    internal interface IResponseTransformer
    {
        object Transform(ActionConfiguration actionConfiguration, object payload);

        bool CanTransform(object data);
    }
}
