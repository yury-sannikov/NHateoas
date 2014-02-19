using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHateoas.Sample.Models.EntityFramework
{
    public class DbProductDetail : Models.ProductDetails
    {
        public DbProductDetail()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        public DbProductDetail(Models.ProductDetails productDetails)
        {
            Id = productDetails.Id;
            ProductId = productDetails.ProductId;
            Details = productDetails.Details;
        }
    }
}