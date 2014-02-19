using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace NHateoas.Sample.Models.EntityFramework
{
    /// <summary>
    /// Product DB model
    /// </summary>
    public class DbProduct : Models.Product
    {
        public DbProduct()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        public DbProduct(Models.Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
        }

        [JsonIgnore]
        public virtual ICollection<DbProductDetail> ProductDetails { get; set; } 
    }
}