using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
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
using NHateoas.Sample.Models.EntityFramework;
using WebGrease.Css.Extensions;
using Product = NHateoas.Sample.Models.Product;

namespace NHateoas.Sample.Controllers
{

    /// <summary>
    /// ProductsController provides CRUD operations against Product class
    /// </summary>
    [HypermediaSource]
    [RoutePrefix("api/Product")]
    public class ProductsController : ApiController, IHypermediaApiControllerConfigurator
    {
        private  readonly DatabaseContext _dbContext = new DatabaseContext();

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
                    .MapEmbeddedEntity<Models.ProductDetails, ProductDetailsController>(model => model.ProductDetailsFromModel,
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
                
                .For((model, controller) => controller.Get(QueryParameter.Is<string>(), QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                    .UseSirenSpecification()
                    .Map((model, controller) => controller.Get())
                        .AsSelfLink()
                    .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                        .AsAction()

            .Configure();
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
            base.Dispose(disposing);
        }
        /// <summary>
        /// Get products collection
        /// </summary>
        /// <returns>Product collection</returns>
        [Hypermedia]
        [Route("")]
        public IEnumerable<Product> Get()
        {
            return _dbContext.Products.ToList();
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
            var res = (from p in _dbContext.Products
                where p.Name.Contains(query)
                orderby p.Name
                select p).Skip(skip);
            if (limit == 0)
                return res.ToList();
            return res.Take(limit).ToList();
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
            var prod = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            var pd = new List<NHateoas.Sample.Models.ProductDetails>();
            pd.AddRange(prod.ProductDetails);
            prod.ProductDetailsFromModel = pd;
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
            var prod = _dbContext.Products.FirstOrDefault(p => p.Name == name);
            
            return Request.CreateResponse<Product>(HttpStatusCode.OK, prod);
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
            var prod = new NHateoas.Sample.Models.EntityFramework.DbProduct(product);
            _dbContext.Products.Add(prod);
            _dbContext.SaveChanges();
            return Request.CreateResponse<Product>(HttpStatusCode.Created, prod);
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
            var prod = _dbContext.Products.FirstOrDefault(p => p.Id == product.Id);
            if (prod == null)
            {
                Request.CreateResponse(HttpStatusCode.NotFound);
                return;
            }
            prod.Name = product.Name ?? prod.Name;
            prod.Price = product.Price;
            _dbContext.SaveChanges();

        }

        /// <summary>
        /// Delete product by ID 
        /// </summary>
        /// <param name="id">Product ID</param>
        [Route("{id:int}")]
        [Hypermedia]
        public HttpResponseMessage Delete(int id)
        {
            var prod = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            if (prod == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            _dbContext.Products.Remove(prod);
            _dbContext.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}