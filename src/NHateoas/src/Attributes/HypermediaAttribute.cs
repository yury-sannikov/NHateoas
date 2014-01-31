using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Threading.Tasks;
using System.Net.Http;
using NHateoas.Configuration;
using NHateoas.Dynamic;
using NHateoas.Dynamic.Strategies;
using NHateoas.Dynamic.Visitors;
using NHateoas.Routes;
using TypeBuilder = NHateoas.Dynamic.TypeBuilder;

namespace NHateoas.Attributes
{
    public class Payload
    {
        public int Data2 = 1;
    }
    
    //[KnownType(typeof(Payload))]
    public class PayloadDecorator
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class HypermediaAttribute : ActionFilterAttribute 
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response == null || actionExecutedContext.Response.Content == null)
                return;

            if (actionExecutedContext.Exception != null)
                return;

            //---- Getting configuration
            var actionDescriptor = actionExecutedContext.ActionContext.ActionDescriptor.ActionBinding.ActionDescriptor as
                ReflectedHttpActionDescriptor;
            
            var controllerType = actionExecutedContext.ActionContext.ControllerContext.Controller.GetType();

            if (actionDescriptor == null)
            {
                Debug.Write(string.Format("Unable to get action descriptor for controller {0}", controllerType.ToString()));
                return;
            }
            var actionConfiguration = HypermediaControllerConfiguration.Instance.GetcontrollerActionConfiguration(controllerType, actionDescriptor.MethodInfo);


            // --- Setup routes first call
            if (!actionConfiguration.RulesHasBeenBuilt)
            {
                var apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();

                RoutesBuilder.Build(controllerType, actionConfiguration, apiExplorer);
            }

            var objectContent = (ObjectContent) actionExecutedContext.Response.Content;

            var payload = objectContent.Value;
            
            var responseTransformer =  actionConfiguration.ResponseTransformerFactory.Get(payload);

            var transformed = responseTransformer.Transform(actionConfiguration, payload);

            actionExecutedContext.Response.Content = new ObjectContent(transformed.GetType(), transformed, objectContent.Formatter);
        }
    }
}
