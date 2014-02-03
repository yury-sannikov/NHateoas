using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace NHateoas.I12n
{
    /// <summary>
    /// Override DefaultHttpControllerTypeResolver to search for and invoke IHypermediaApiControllerConfigurator
    /// Controller itself can implement IHypermediaApiControllerConfigurator if it has empty default constructor
    /// </summary>
    internal class HypermediaInitializerTypeResolver : DefaultHttpControllerTypeResolver
    {
        public HypermediaInitializerTypeResolver()
            : base(IsControllerInitializerType)
        {
        }
        
        internal static bool IsControllerInitializerType(Type t)
        {
            Contract.Assert(t != null);
            return
                t != null &&
                t.IsClass &&
                !t.IsAbstract &&
                typeof(IHypermediaApiControllerConfigurator).IsAssignableFrom(t);
        }

    }
}
