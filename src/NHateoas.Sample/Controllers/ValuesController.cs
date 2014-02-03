using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NHateoas.Sample.Models;
using NHateoas.Sample.Models;

namespace NHateoas.Sample.Controllers
{

    [HypermediaSource]
    public class ValuesController : ApiController, IHypermediaApiControllerConfigurator
    {
        private static readonly Product[] Products =
        {
            new Product() { Id = 1, Name = "Item1", Price = 2.99m } ,
            new Product() { Id = 2, Name = "Item2", Price = 3.99m } ,
            new Product() { Id = 3, Name = "Item3", Price = 4.99m } ,
            new Product() { Id = 4, Name = "Item4", Price = 5.99m } ,
            new Product() { Id = 5, Name = "Item5", Price = 6.99m } 
        
        };

        public void ConfigureHypermedia()
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
                            new {method = "GET"})

                .For((model, controller) => controller.Get(model.Name))
                    .Map((model, controller) => controller.Get(model.Id))
                    .Map((model, controller) => controller.Get(model.Name))
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                .For((model, controller) => controller.Get())
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

        [Hypermedia]
        public HttpResponseMessage Get(string name)
        {
            return Request.CreateResponse<Product>(HttpStatusCode.Unauthorized, Products.First());
        }

        // POST api/values
        public void Post([FromBody] Product product)
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