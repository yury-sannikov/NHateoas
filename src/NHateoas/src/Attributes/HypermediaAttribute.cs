using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Threading.Tasks;
using System.Net.Http;
using NHateoas.Dynamic;
using NHateoas.Dynamic.Strategies;
using NHateoas.Dynamic.Visitors;
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
            var objectContent = (ObjectContent) actionExecutedContext.Response.Content;
            
            var payload = objectContent.Value;

            var strategyBuilder = new StrategyBuilder()
                .For(payload.GetType())
                .WithSimpleProperties();

            var strategy = strategyBuilder.Build();

            var typeBuilder = new TypeBuilder(payload.GetType(), strategy);
            
            var proxyType = typeBuilder.BuildType();

            var newinstance = Activator.CreateInstance(proxyType);

            strategy.ActivateInstance(newinstance, payload, new Dictionary<string, string>());

            actionExecutedContext.Response.Content = new ObjectContent(newinstance.GetType(), newinstance, objectContent.Formatter);

            /*
            var objectContent = (ObjectContent) actionExecutedContext.Response.Content;
            var payload = objectContent.Value;

            var actiualType = actionExecutedContext.Response.Content.GetType();
            Type[] actiualTypeParameters = actiualType.GetGenericArguments();


            var tb = new TypeBuilder(payload.GetType(), null);
            //tb.AddVisitor(new PropertyVisitor(typeof(PayloadDecorator).GetProperties()[0]));

            var av = new AggregateTypeBuilder(typeof (PayloadDecorator), "_payloadDecorator");
            av.AddVisitor(new AggregatedPropertyVisitor(typeof (PayloadDecorator).GetProperties()[0]))
              .AddVisitor(new AggregatedPropertyVisitor(typeof (PayloadDecorator).GetProperties()[1]));
            
            tb.AddVisitor(av);

            var newType = tb.BuildType();

           
            //var dg = new DecoratorGenerator();
            //var newtype = dg.GetProxyType(payload.GetType(), payload.GetType());

            var newinstance = Activator.CreateInstance(newType);

            var aggfield = newType.GetField("_payloadDecorator",
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);
            aggfield.SetValue(newinstance, new PayloadDecorator()
            {
                Id = 99,
                Name = "Test"
            });

            //var prop = newType.GetProperty("Data");
            //prop.SetValue(newinstance, 10, null);

            //var payloadDec = new PayloadDecorator();// {Object = payload};
            //dynamic payloadDec = new ExpandoObject();
            //payloadDec.Test = 1;

            //TypeDescriptor.AddAttributes(payloadDec, new KnownTypeAttribute(typeof(Payload)));

            //var objectContextDecorated = Activator.CreateInstance(actiualType, payload, objectContent.Formatter);

            //actionExecutedContext.Response.Content = (ObjectContent)objectContextDecorated;
            //actionExecutedContext.Response.Content = new ObjectContent(payloadDec.GetType(), payloadDec, objectContent.Formatter);
            //actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, objectContent.Value);
            //actionExecutedContext.Response.Content = new ObjectContent(payload.GetType(), payload, objectContent.Formatter);
            actionExecutedContext.Response.Content = new ObjectContent(newinstance.GetType(), newinstance, objectContent.Formatter);


            //actiualType.MakeGenericType();

            //var res =
            //    ((System.Net.Http.ObjectContent<System.Collections.Generic.IEnumerable<NHateoas.Sample.Models.Product>>)
            //        actionExecutedContext.Response.Content).Value;

            //actionExecutedContext.Response
            //actionExecutedContext.Response = new HttpResponseMessage();
            //var request = new HttpRequestMessage<Product>();
            //ObjectContent<T> content = request.CreateContent<T>(operationInput, new MediaTypeHeaderValue(Constants.ContentTypeXml), new MediaTypeFormatterCollection() { new XmlMediaTypeFormatter() }, new FormatterSelector());
            */
        }
    }
}
