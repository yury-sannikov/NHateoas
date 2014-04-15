using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Routes
{
    internal interface IMetadataProvider
    {
        object GetMetadataByType(Type metadataType, params object[] values);
        
        IList<Type> GetRegisteredMetadataTypes();

        string ContentType { get; }
    }
}
