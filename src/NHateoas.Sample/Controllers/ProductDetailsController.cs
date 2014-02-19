using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NHateoas.Sample.Models;
using NHateoas;
using NHateoas.Sample.Models.EntityFramework;

namespace NHateoas.Sample.Controllers
{
    public class ProductDetailsControllerHypermediaConfigurator : IHypermediaApiControllerConfigurator
    {
        public void ConfigureHypermedia(HttpConfiguration httpConfiguration)
        {
            new HypermediaConfigurator<ProductDetails, ProductDetailsController>(httpConfiguration)
                .For((model, controller) => controller.Get(model.Id))
                    .UseSirenSpecification()    
                    .Map((model, controller) => controller.GetByProductId(model.ProductId))
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                    .MapReference<ProductsController>((model, other) => other.Get(model.ProductId))
                        .AsParentLink()
                .For((model, controller) => controller.GetByProductId(model.ProductId))
                    .UseSirenSpecification()
                    .Map((model, controller) => controller.GetByProductId(model.ProductId))
                    .Map((model, controller) => controller.Get(model.Id))
                        .AsSelfLink()
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                    .MapReference<ProductsController>((model, other) => other.Get(model.ProductId))
                        .AsParentLink()
                .Configure();
        }
    }

    [HypermediaSource]
    [RoutePrefix("api/ProductDetails")]
    public class ProductDetailsController : ApiController
    {
        private readonly DatabaseContext _dbContext = new DatabaseContext();

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
            base.Dispose(disposing);
        }

        // GET api/productdetails
        [Route("")]
        [Hypermedia]
        public IEnumerable<ProductDetails> Get()
        {
            return _dbContext.ProductDetails.ToList();
        }

        // GET api/productdetails/5
        [Route("{id:int}")]
        [Hypermedia]
        public ProductDetails Get(int id)
        {
            return _dbContext.ProductDetails.FirstOrDefault(p => p.Id == id);
        }

        [Route("~/api/Product/{id}/Details")]
        [Hypermedia]
        public IEnumerable<ProductDetails> GetByProductId(int id)
        {
            return _dbContext.ProductDetails.Where(p => p.ProductId == id).ToList();
        }

        // POST api/productdetails
        [Route("")]        
        public void Post([FromBody]ProductDetails value)
        {
        }

        // PUT api/productdetails/5
        [Route("{id:int}")]
        public void Put(int id, [FromBody]ProductDetails value)
        {
        }

        // DELETE api/productdetails/5
        [Route("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}
