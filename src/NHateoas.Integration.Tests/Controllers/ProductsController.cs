using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NHateoas.Integration.Tests.Models;

namespace NHateoas.Integration.Tests.Controllers
{

    /// <summary>
    /// ProductsController provides CRUD operations against Product class
    /// </summary>
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

        public void ConfigureHypermedia(HttpConfiguration httpConfiguration)
        {
            // Set up model-controller mapping.
            new HypermediaConfigurator<Product, ProductsController>(httpConfiguration)
                .For((model, controller) => controller.Get(model.Id))
                    .UseSirenSpecification()
                    .Map((model, controller) => controller.Get(model.Id))
                        .AsSelfLink()
                    .Map((model, controller) => controller.Get())
                        .AsParentLink()
                    .Map((model, controller) => controller.Get(QueryParameter.Is<string>(), QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                        .AsAction()
                    .Map((model, controller) => controller.Get(model.Name))
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                    .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                        .AsLink()
                    .MapEmbeddedEntity<ProductDetails, ProductDetailsController>(model => model.ProductDetailsFromModel,
                        (model, controller) => controller.GetByProductId(model.Id))
                    
                 .For((model, controller) => controller.Get(model.Name))
                    .UseSirenSpecification()
                    .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                        .AsLink()
                    .Map((model, controller) => controller.Get())
                        .AsParentLink()
                    .Map((model, controller) => controller.Get(QueryParameter.Is<string>(), QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                        .AsAction()
                    .Map((model, controller) => controller.Get(model.Id))
                    .Map((model, controller) => controller.Get(model.Name))
                        .AsSelfLink()
                    .Map((model, controller) => controller.Post(model))
                    .Map((model, controller) => controller.Put(model.Id, model))
                    .Map((model, controller) => controller.Delete(model.Id))
                    
                .For((model, controller) => controller.Get())
                    .UseSirenSpecification()
                    .Map((model, controller) => controller.Get())
                        .AsSelfLink()
                    .Map((model, controller) => controller.Get(model.Id))
                    .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                        .AsAction()

                .For((model, controller) => controller.Post(model))
                    .UseSirenSpecification()
                    .Map((model, controller) => controller.Get(model.Id))
                    .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))

            .Configure();
        }

        /// <summary>
        /// Get products collection
        /// </summary>
        /// <returns>Product collection</returns>
        [Hypermedia]
        [Route("")]
        public IEnumerable<Product> Get()
        {
            return Products;
        }
        
        /// <summary>
        /// Search for products by query and do pagination using skip and limit parameters
        /// </summary>
        /// <param name="query">Query to be executed</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="limit">Result limit</param>
        /// <returns></returns>
        [Hypermedia]
        [Route("")]
        public IEnumerable<Product> Get(string query, int skip, int limit)
        {
            return Products;
        }

        /// <summary>
        /// Get product by product id
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product instance</returns>
        [Hypermedia(Names = new []{"get-by-id"})]
        [Route("{id:int}")]
        public Product Get(int id)
        {
            var prod = Products.First();
            prod.ProductDetailsFromModel = new List<ProductDetails>()
            {
                new ProductDetails()
                {
                    Details = "D1",
                    Id = 1,
                    ProductId = 1
                }
            };
            return prod;
        }
        
        /// <summary>
        /// Get first product by produt name
        /// </summary>
        /// <param name="name">Product name to search</param>
        /// <returns>First product with mathing name</returns>
        [Route("{name}")]
        [ResponseType(typeof(Product))]
        [Hypermedia]
        public HttpResponseMessage Get(string name)
        {
            return Request.CreateResponse<Product>(HttpStatusCode.OK, Products.First());
        }


        /// <summary>
        /// Create new product and return created object back with database generated ID
        /// </summary>
        /// <param name="product">Product to create</param>
        /// <returns>Product with database identifier</returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Product))]
        [Hypermedia(Names = new []{"create-product"})]
        public HttpResponseMessage Post([FromBody]Product product)
        {
            return Request.CreateResponse<Product>(HttpStatusCode.Created, Products.First());
        }

        /// <summary>
        /// Modify existing product objects
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="product">Updated product</param>
        [HttpPut]
        [Route("{id:int}")]
        [Hypermedia]
        public void Put(int id, [FromBody]Product product)
        {
        }

        /// <summary>
        /// Delete product by ID 
        /// </summary>
        /// <param name="id">Product ID</param>
        [Route("{id:int}")]
        [Hypermedia]
        public void Delete(int id)
        {
        }
    }
}