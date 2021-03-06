﻿using System;
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
        public static string MakeAbsolutePath(string relativePath)
        {
            var virtualRoot = System.Web.Http.GlobalConfiguration.Configuration.VirtualPathRoot ?? string.Empty;

            var builder = new UriBuilder();

            if (!relativePath.StartsWith("/"))
            {
                builder.Path = virtualRoot;
            }


            var actionExecutedContext = ActionCallContext.Get<HttpActionExecutedContext>();

            if (actionExecutedContext != null)
            {
                var requestUri = actionExecutedContext.Request.RequestUri;

                builder.Scheme = requestUri.Scheme;
                builder.Host = requestUri.Host;
                if (!requestUri.IsDefaultPort)
                    builder.Port = requestUri.Port;
            }
            
            var result = builder.ToString();

            if (string.IsNullOrEmpty(relativePath))
                return result;

            if (!result.EndsWith("/"))
                result += "/";

            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Remove(0, 1);
            
            result += relativePath;

            return result;
        }
    }
}
