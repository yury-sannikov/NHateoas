using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using NHateoas.I12n;

namespace NHateoas
{
    public static class HypermediaInitializer
    {
        public static void InitializeHypermedia(this HttpConfiguration configuration)
        {
            IAssembliesResolver assembliesResolver = configuration.Services.GetAssembliesResolver();

            var initResolver = new HypermediaInitializerTypeResolver();

            var resolvedInitializers = initResolver.GetControllerTypes(assembliesResolver);

            resolvedInitializers.ToList().ForEach(t => ((IHypermediaApiControllerConfigurator)Activator.CreateInstance(t)).ConfigureHypermedia());
        }
    }
}
