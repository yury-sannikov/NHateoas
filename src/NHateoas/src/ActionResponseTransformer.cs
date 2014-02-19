using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NHateoas.Configuration;

namespace NHateoas
{
    [SecuritySafeCritical]
    internal static class ActionResponseTransformer
    {
        public static ObjectContent Transform(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response == null || actionExecutedContext.Response.Content == null)
                return null;

            if (actionExecutedContext.Exception != null)
                return null;

            if (!(actionExecutedContext.Response.Content is ObjectContent))
                return null;

            var actionDescriptor = actionExecutedContext.ActionContext.ActionDescriptor.ActionBinding.ActionDescriptor as
                ReflectedHttpActionDescriptor;

            var controllerType = actionExecutedContext.ActionContext.ControllerContext.Controller.GetType();

            if (actionDescriptor == null)
            {
                Debug.Write(string.Format("Unable to get action descriptor for controller {0}", controllerType));
                return null;
            }

            var actionConfiguration = HypermediaControllerConfiguration.Instance.GetcontrollerActionConfiguration(controllerType, actionDescriptor.MethodInfo);

            if (actionConfiguration == null)
                return null;

            var objectContent = (ObjectContent)actionExecutedContext.Response.Content;

            var payload = objectContent.Value;

            if (payload == null)
                return null;

            var responseTransformer = actionConfiguration.ResponseTransformerFactory.Get(payload);

            if (responseTransformer == null)
            {
                throw new Exception(string.Format("Unable to get response transformer for response type {0}", payload.GetType()));
            }

            var transformed = responseTransformer.Transform(actionConfiguration, payload);

            return new ObjectContent(transformed.GetType(), transformed, objectContent.Formatter);
        }

    }
}
