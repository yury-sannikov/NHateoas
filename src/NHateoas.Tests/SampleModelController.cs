using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using NHateoas.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Web;


namespace NHateoas.Tests
{
    public class ModelSample
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        
        [DataMember(Name = "email_address")]
        [DataType(DataType.EmailAddress)]
        public string EMailAddress { get; set; }
    }

    public class ControllerSample
    {
        public ModelSample ControllerMethod(int id, string name, string query, int skip)
        {
            return null;
        }

        public IEnumerable<ModelSample> ControllerQueryMethod(int id, string name, string query, int skip)
        {
            return null;
        }

        public ModelSample ControllerMethodPut(int id, [FromBody] ModelSample model)
        {
            return null;
        }

        public void ControllerVoidMethod()
        {
            
        }
        
        public HttpResponseMessage ControllerHttpResponseMessageMethod()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        
        [ResponseType(typeof(string))]
        public HttpResponseMessage ControllerHttpResponseMessageMethodWithType()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage ControllerHttpResponseMessageMethodParam(int id)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [ResponseType(typeof(string))]
        public HttpResponseMessage ControllerHttpResponseMessageMethodWithTypeParam(double price)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public static int SomeMethod()
        {
            return 2;
        }

        [Hypermedia(Names = new[] { "rel-name" })]
        public int FakeMethodWithAttribute()
        {
            return 0;
        }
        
        public int FakeMethod()
        {
            return 0;
        }
    }
}
