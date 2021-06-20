using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Repository.SQL
{
    public class StoreContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; }

        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ProductConfig(builder);
        }

        private void ProductConfig(ModelBuilder builder)
        {
            builder.Entity<Product>(e =>
            {
                e.ToTable("Products");
                e.HasKey(p => p.Id).HasName("Id");
                e.Property(p => p.Description).HasColumnName("Description");
                e.Property(p => p.Price).HasColumnName("Price").HasColumnType("numeric(20,8)");                
            });
        }
    }
}
