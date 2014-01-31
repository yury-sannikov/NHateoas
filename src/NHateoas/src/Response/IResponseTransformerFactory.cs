using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Response
{
    /// <summary>
    /// Return response transformer by specified object
    /// </summary>
    internal interface IResponseTransformerFactory
    {
        IResponseTransformer Get(object data);
    }
}
