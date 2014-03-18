using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class QueryParameter
    {
        public static TValue Is<TValue>()
        {
            return default(TValue);
        }
    }
}
