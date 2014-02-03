using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    public static class QueryParameter
    {
        public static TValue Is<TValue>()
        {
            return default(TValue);
        }
    }
}
