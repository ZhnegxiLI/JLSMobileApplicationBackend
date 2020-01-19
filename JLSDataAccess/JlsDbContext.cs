using JLSDataModel.Models;
using JLSDataModel.Models.Order;
using JLSDataModel.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace JLSDataAccess
{
    public class JlsDbContext : DbContext
    {
        public JlsDbContext(DbContextOptions<JlsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ReferenceCategory> ReferenceCategory { get; set; }

        public virtual DbSet<ReferenceItem> ReferenceItem { get; set; }

        public virtual DbSet<ReferenceLabel> ReferenceLabel { get; set; }

        public virtual DbSet<OrderInfo> OrderInfo { get; set; }
        public virtual DbSet<OrderInfoLog> OrderInfoLog { get; set; }

        public virtual DbSet<OrderInfoShipping> OrderInfoShipping { get; set; }


        public virtual DbSet<DiscountActivity> DiscountActivity { get; set; }

        public virtual DbSet<DiscountActivityProduct> DiscountActivityProduct { get; set; }

        public virtual DbSet<Product> Product { get; set; }

        public virtual DbSet<ProductPhotoPath> ProductPhotoPath { get; set; }
    }
}
