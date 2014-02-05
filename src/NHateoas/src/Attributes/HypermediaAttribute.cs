using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using NHateoas.Configuration;
using NHateoas.Routes;

namespace NHateoas.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class HypermediaAttribute : ActionFilterAttribute
    {
        private readonly List<string> _rels = new List<string>();
        public HypermediaAttribute()
        {
        }

        public HypermediaAttribute(string rel)
        {
            _rels.Add(rel);
        }

        public HypermediaAttribute(string[] rels)
        {
            _rels.AddRange(rels.ToList());
        }

        public List<string> Rels
        {
            get { return _rels; }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response == null || actionExecutedContext.Response.Content == null)
                return;

            if (actionExecutedContext.Exception != null)
                return;

            if (!(actionExecutedContext.Response.Content is ObjectContent))
                return;

            var actionDescriptor = actionExecutedContext.ActionContext.ActionDescriptor.ActionBinding.ActionDescriptor as
                ReflectedHttpActionDescriptor;
            
            var controllerType = actionExecutedContext.ActionContext.ControllerContext.Controller.GetType();

            if (actionDescriptor == null)
            {
                Debug.Write(string.Format("Unable to get action descriptor for controller {0}", controllerType));
                return;
            }

            var actionConfiguration = HypermediaControllerConfiguration.Instance.GetcontrollerActionConfiguration(controllerType, actionDescriptor.MethodInfo);

            if (actionConfiguration == null)
                return;

            var objectContent = (ObjectContent) actionExecutedContext.Response.Content;

            var payload = objectContent.Value;
            
            var responseTransformer =  actionConfiguration.ResponseTransformerFactory.Get(payload);

            if (responseTransformer == null)
            {
                throw new Exception(string.Format("Unable to get response transformer for response type {0}", payload.GetType()));
            }

            var transformed = responseTransformer.Transform(actionConfiguration, payload);

            actionExecutedContext.Response.Content = new ObjectContent(transformed.GetType(), transformed, objectContent.Formatter);
        }
    }
}
