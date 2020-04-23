using JLSDataModel.Models;
using JLSDataModel.Models.Adress;
using JLSDataModel.Models.Audit;
using JLSDataModel.Models.Message;
using JLSDataModel.Models.Order;
using JLSDataModel.Models.Product;
using JLSDataModel.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JLSDataAccess
{
    public class JlsDbContext : IdentityDbContext<User, IdentityRole<int>, int>
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


        public virtual DbSet<DiscountActivity> DiscountActivity { get; set; }

        public virtual DbSet<DiscountActivityProduct> DiscountActivityProduct { get; set; }

        public virtual DbSet<Product> Product { get; set; }

        public virtual DbSet<ProductPhotoPath> ProductPhotoPath { get; set; }
        public virtual DbSet<UserPreferenceCategory> UserPreferenceCategory { get; set; }

        public virtual DbSet<Adress> Adress { get; set; }

        public virtual DbSet<CustomerInfo> CustomerInfo { get; set; }

        public virtual DbSet<UserShippingAdress> UserShippingAdress { get; set; }

        public virtual DbSet<Audit> Audit { get; set; }
        public virtual DbSet<AuditData> AuditData { get; set; }

        public virtual DbSet<DeletedRecords> DeletedRecords { get; set; }
        public virtual DbSet<OrderProduct> OrderProduct { get; set; }

        public virtual DbSet<ProductComment> ProductComment { get; set; }

        public virtual DbSet<TokenModel> TokenModel { get; set; }

        public virtual DbSet<OrderInfoStatusLog> OrderInfoStatusLog { get; set; }

        public virtual DbSet<Message> Message { get; set; }

        public virtual DbSet<MessageDestination> MessageDestination { get; set; }
        public virtual DbSet<Remark> Remark { get; set; }
        public virtual DbSet<ShipmentInfo> ShipmentInfo { get; set; }

        public virtual DbSet<ProductSearchCount> ProductSearchCount { get; set; }
        public virtual DbSet<ProductVisitCount> ProductVisitCount { get; set; }

        public virtual DbSet<ProductFavorite> ProductFavorite { get; set; }

        public virtual DbSet<UserCountInfo> UserCountInfo { get; set; }

        public virtual DbSet<ExportConfiguration> ExportConfiguration { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<EmailToSend> EmailToSend { get; set; }
    }
}
