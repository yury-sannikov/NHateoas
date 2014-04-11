using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NHateoas.Integration.Tests.Models;

namespace NHateoas.Integration.Tests.Controllers
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
                    .Map((model, controller) => controller.Get(model.Id))
                        .AsSelfLink()
                .For((model, controller) => controller.GetByProductId(model.ProductId)).UseSirenSpecification()
                    .Map((model, controller) => controller.GetByProductId(model.ProductId))
                        .AsSelfLink()
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                // put has no 'self' link
                .For((model, controller) => controller.Put(model.ProductId, model)).UseSirenSpecification()
                    .Map((model, controller) => controller.Put(model.Id, model))
                .Configure();
        }
    }

    [HypermediaSource]
    [RoutePrefix("api/ProductDetails")]
    public class ProductDetailsController : ApiController
    {
        private static readonly ProductDetails[] Details = { new ProductDetails() { Id = 1, ProductId = 1, Details = "Cup details" } };

        // GET api/productdetails
        [Route("")]
        public IEnumerable<ProductDetails> Get()
        {
            return Details;
        }

        [Hypermedia]
        [Route("{id:int}")]
        public ProductDetails Get(int id)
        {
            return Details.First();
        }

        [Hypermedia]
        [Route("~/api/Product/{id}/Details")]
        public ProductDetails GetByProductId(int id)
        {
            return Details.First();
        }

        // POST api/productdetails
        [Route("")]        
        public void Post([FromBody]ProductDetails value)
        {
        }

        // PUT api/productdetails/5
        [Route("{id:int}")]
        public ProductDetails Put(int id, [FromBody]ProductDetails value)
        {
            return value;
        }

        // DELETE api/productdetails/5
        [Route("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}
