using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHateoas.Integration.Tests.Models
{
    
    /// <summary>
    /// This class represent Product
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }

        [JsonIgnore]
        public IEnumerable<ProductDetails> ProductDetailsFromModel { get; set; }

    }
}