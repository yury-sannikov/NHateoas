using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.ModelBinding;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NHateoas.Sample.Models;

namespace NHateoas.Sample.Controllers
{

    [HypermediaSource]
    [RoutePrefix("api/Product")]
    public class ProductsController : ApiController, IHypermediaApiControllerConfigurator
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
            new HypermediaConfigurator<Product, ProductsController>()
                .For((model, controller) => controller.Get(model.Id))
                    .UseSirenSpecification()
                        .Map((model, controller) => controller.Get())
                        .Map((model, controller) => controller.Get(model.Id))
                        .Map((model, controller) => controller.Get(QueryParameter.Is<string>(), QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                        .Map((model, controller) => controller.Get(model.Name))
                        .Map((model, controller) => controller.Post(model))
                        .Map((model, controller) => controller.Put(model.Id, model))
                        .Map((model, controller) => controller.Delete(model.Id))
                        .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                    
                 .For((model, controller) => controller.Get(model.Name))
                    .UseSirenSpecification()
                        .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                        .Map((model, controller) => controller.Get())
                        .Map((model, controller) => controller.Get(QueryParameter.Is<string>(), QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                        .Map((model, controller) => controller.Get(model.Id))
                        .Map((model, controller) => controller.Get(model.Name))
                        .Map((model, controller) => controller.Post(model))
                        .Map((model, controller) => controller.Put(model.Id, model))
                        .Map((model, controller) => controller.Delete(model.Id))
                    
                .For((model, controller) => controller.Get())
                    .UseSirenSpecification()
                        .Map((model, controller) => controller.Get(model.Id))
                        .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))

                .For((model, controller) => controller.Post(model))
                    .UseSirenSpecification()
                        .Map((model, controller) => controller.Get(model.Id))
                        .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))

            .Configure();
        }

        [Hypermedia]
        [Route("")]
        public IEnumerable<Product> Get()
        {
            return Products;
        }
        
        [Hypermedia]
        [Route("")]
        public IEnumerable<Product> Get(string query, int skip, int limit)
        {
            return Products;
        }
        
        [Hypermedia]
        [Route("{id:int}")]
        public Product Get(int id)
        {
            return Products.First();
        }
        
        [Route("{name}")]
        [ResponseType(typeof(Product))]
        [Hypermedia]
        public HttpResponseMessage Get(string name)
        {
            return Request.CreateResponse<Product>(HttpStatusCode.Unauthorized, Products.First());
        }

        // POST api/values
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Product))]
        [Hypermedia]
        public HttpResponseMessage Post([FromBody]Product product)
        {
            return Request.CreateResponse<Product>(HttpStatusCode.Created, Products.First());
        }

        // PUT api/values/5
        [HttpPut]
        [Route("{id:int}")]
        public void Put(int id, [FromBody]Product product)
        {
        }

        // DELETE api/values/5
        [Route("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}