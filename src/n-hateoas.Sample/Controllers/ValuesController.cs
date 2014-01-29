using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NHateoas.Sample.Models;

namespace NHateoas.Sample.Controllers
{
    [HypermediaSource]
    public class ValuesController : ApiController
    {
        private static readonly Product[] Products = { new Product() { Id = 1, Name = "Cup", Price = 2.99m } };

        static ValuesController()
        {
            // Set up model-controller mapping.
            new HypermediaConfigurator<Product, ValuesController>()
                .Map((model, controller) => controller.Get())
                .Map((model, controller) => controller.Get(model.Id))
            .Configure();
        }

        [Hypermedia]
        public IEnumerable<Product> Get()
        {
            return Products;
        }

        [Hypermedia]
        public Product Get(int id)
        {
            return Products.First();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}