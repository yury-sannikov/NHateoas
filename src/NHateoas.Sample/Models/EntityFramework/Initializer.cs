using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NHateoas.Sample.Models.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class Initializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            var products = new List<DbProduct>()
            {
                new DbProduct()
                {
                    Name = "Teapot",
                    Price = 25.54m,
                    ProductDetails = new Collection<DbProductDetail>()
                    {
                        new DbProductDetail(){Details = "Teapod detail 1"},
                        new DbProductDetail(){Details = "Teapod detail 2"}
                    }
                },
                new DbProduct()
                {
                    Name = "Spoon set",
                    Price = 45.65m,
                    ProductDetails = new Collection<DbProductDetail>()
                    {
                        new DbProductDetail(){Details = "Spoon set detail 1"},
                        new DbProductDetail(){Details = "Spoon set detail 2"}
                    }
                },
                new DbProduct()
                {
                    Name = "Dining Table",
                    Price = 125m,
                    ProductDetails = new Collection<DbProductDetail>()
                    {
                        new DbProductDetail(){Details = "Dining Table detail 1"},
                        new DbProductDetail(){Details = "Dining Table detail 2"},
                        new DbProductDetail(){Details = "Dining Table detail 3"}
                    }
                },
                new DbProduct()
                {
                    Name = "Small Chair",
                    Price = 48m
                },
                new DbProduct()
                {
                    Name = "Tall Chair",
                    Price = 119.4m
                }
            };
            
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}