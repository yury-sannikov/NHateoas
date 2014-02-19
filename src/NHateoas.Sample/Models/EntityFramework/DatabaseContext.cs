using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NHateoas.Sample.Models.EntityFramework
{
    /// <summary>
    /// Entity Framework database context
    /// </summary>
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Products table
        /// </summary>
        public DbSet<DbProduct> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.Entity<DbProduct>()
                .HasKey(p => p.Id);
        }
    }
}