using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    internal static class LinkHelper
    {
        public static string MakeAbsolutePath(ApiDescription apiDescription)
        {
            var virtualRoot = System.Web.Http.GlobalConfiguration.Configuration.VirtualPathRoot ?? string.Empty;

            if (!string.IsNullOrEmpty(virtualRoot) && !virtualRoot.EndsWith("/"))
                virtualRoot = virtualRoot + "/";

            var builder = new UriBuilder
            {
                Path =
                    string.Format("{0}{1}", virtualRoot, apiDescription.RelativePath)
            };

            var actionExecutedContext = ActionCallContext.Get<HttpActionExecutedContext>();

            if (actionExecutedContext != null)
            {
                var requestUri = actionExecutedContext.Request.RequestUri;

                builder.Scheme = requestUri.Scheme;
                builder.Host = requestUri.Host;
                if (!requestUri.IsDefaultPort)
                    builder.Port = requestUri.Port;
            }

            return builder.ToString();
        }
    }
}
