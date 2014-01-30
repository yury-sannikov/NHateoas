using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHateoas.Attributes;
using NHateoas.Configuration;
using n_hateoas.Sample.Models;

namespace n_hateoas.Sample.Controllers
{
    [HypermediaSource]
    public class ProductDetailsController : ApiController
    {
        private static readonly ProductDetails[] Details = { new ProductDetails() { Id = 1, ProductId = 1, Details = "Cup details" } };

        public ProductDetailsController()
        {
            new HypermediaConfigurator<ProductDetails, ProductDetailsController>()
                .For((model, controller) => controller.Get(model.Id))
                    .Map((model, controller) => controller.GetByProductId(model.ProductId))
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id));

        }

        // GET api/productdetails
        public IEnumerable<ProductDetails> Get()
        {
            return Details;
        }

        // GET api/productdetails/5
        public ProductDetails Get(int id)
        {
            return Details.First();
        }

        [ActionName("DetailsByProductId")]
        public ProductDetails GetByProductId(int id)
        {
            return Details.First();
        }

        // POST api/productdetails
        public void Post([FromBody]ProductDetails value)
        {
        }

        // PUT api/productdetails/5
        public void Put(int id, [FromBody]ProductDetails value)
        {
        }

        // DELETE api/productdetails/5
        public void Delete(int id)
        {
        }
    }
}
