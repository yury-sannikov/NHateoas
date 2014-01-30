using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NHateoas.Sample.Models;
using n_hateoas.Sample.Models;

namespace NHateoas.Sample.Controllers
{
    [HypermediaSource]
    public class ValuesController : ApiController
    {
        private static readonly Product[] Products = { new Product() { Id = 1, Name = "Cup", Price = 2.99m } };

        public ValuesController()
        {
            // Set up model-controller mapping.
            new HypermediaConfigurator<Product, ValuesController>()
                .For((model, controller) => controller.Get(model.Id))
                    .Map((model, controller) => controller.Get(model.Name))
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                        .MapReference<ProductDetails>(thisModel => thisModel.Id, 
                            thatModel => thatModel.ProductId, 
                            new {method = "POST"})

                .For((model, controller) => controller.Get(model.Name))
                    .Map((model, controller) => controller.Get(model.Id))
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                .For((model, controller) => controller.Get())
                    .Map((model, controller) => controller.Get(model.Id))
            .Configure();
        }

        //[Hypermedia]
        public IEnumerable<Product> Get()
        {
            return Products;
        }

        [Hypermedia]
        public Product Get(int id)
        {
            return Products.First();
        }

        [Hypermedia]
        public Product Get(string name)
        {
            return Products.First();
        }

        // POST api/values
        public void Post([FromBody]Product product)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]Product product)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}