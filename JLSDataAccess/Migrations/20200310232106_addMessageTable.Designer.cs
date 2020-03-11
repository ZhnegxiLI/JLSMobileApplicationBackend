﻿// <auto-generated />
using System;
using JLSDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JLSDataAccess.Migrations
{
    [DbContext(typeof(JlsDbContext))]
    [Migration("20200310232106_addMessageTable")]
    partial class addMessageTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("JLSDataModel.Models.Adress.Adress", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City");

                    b.Property<string>("ContactFax");

                    b.Property<string>("ContactFirstName");

                    b.Property<string>("ContactLastName");

                    b.Property<string>("ContactTelephone");

                    b.Property<string>("Country");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("EntrepriseName");

                    b.Property<string>("FirstLineAddress");

                    b.Property<bool?>("IsDefaultAdress");

                    b.Property<string>("Provence");

                    b.Property<string>("SecondLineAddress");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.ToTable("Adress");
                });

            modelBuilder.Entity("JLSDataModel.Models.Audit.Audit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuditType");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("TableName");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("Audit");
                });

            modelBuilder.Entity("JLSDataModel.Models.Audit.AuditData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AuditId");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("FieldName");

                    b.Property<string>("NewValue");

                    b.Property<string>("OldValue");

                    b.Property<long>("RowId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("AuditId");

                    b.ToTable("AuditData");
                });

            modelBuilder.Entity("JLSDataModel.Models.Audit.DeletedRecords", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long?>("DeletedBy");

                    b.Property<long?>("DeletedId");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("TableName");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("DeletedRecords");
                });

            modelBuilder.Entity("JLSDataModel.Models.Message.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("Title");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("JLSDataModel.Models.Message.MessageDestination", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<int?>("FromUserId");

                    b.Property<int?>("ToUserId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("MessageDestination");
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderInfo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdminRemark");

                    b.Property<string>("ClientRemark");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long>("FacturationAdressId");

                    b.Property<string>("OrderReferenceCode");

                    b.Property<string>("OrderType");

                    b.Property<string>("PaymentInfo");

                    b.Property<long>("ShippingAdressId");

                    b.Property<long>("StatusReferenceItemId");

                    b.Property<float?>("TaxRate");

                    b.Property<float?>("TotalPrice");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("FacturationAdressId");

                    b.HasIndex("ShippingAdressId");

                    b.HasIndex("StatusReferenceItemId");

                    b.HasIndex("UserId");

                    b.ToTable("OrderInfo");
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderInfoLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChangedDescription");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long>("OrderInfoId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("OrderInfoId");

                    b.ToTable("OrderInfoLog");
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderInfoStatusLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ActionTime");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long?>("NewStatusId");

                    b.Property<long?>("OldStatusId");

                    b.Property<long>("OrderInfoId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("OrderInfoId");

                    b.ToTable("OrderInfoStatusLog");
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderProduct", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long>("OrderId");

                    b.Property<long?>("OrderInfoId");

                    b.Property<int>("Quantity");

                    b.Property<long>("ReferenceId");

                    b.Property<double?>("TotalPrice");

                    b.Property<double?>("UnitPrice");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("OrderInfoId");

                    b.HasIndex("ReferenceId");

                    b.ToTable("OrderProduct");
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.DiscountActivity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<float>("DiscountPercentage");

                    b.Property<string>("Title");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<bool?>("Validity");

                    b.HasKey("Id");

                    b.ToTable("DiscountActivity");
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.DiscountActivityProduct", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long>("DiscountActivityId");

                    b.Property<long>("ProductId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("DiscountActivityId");

                    b.HasIndex("ProductId");

                    b.ToTable("DiscountActivityProduct");
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.Product", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Color");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<string>("Material");

                    b.Property<int?>("MinQuantity");

                    b.Property<float?>("Price");

                    b.Property<int?>("QuantityPerBox");

                    b.Property<long>("ReferenceItemId");

                    b.Property<string>("Size");

                    b.Property<float?>("TaxRate");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("ReferenceItemId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.ProductComment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<int>("Level");

                    b.Property<long>("ProductId");

                    b.Property<string>("Title");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("ProductComment");
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.ProductPhotoPath", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("Path");

                    b.Property<long>("ProductId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductPhotoPath");
                });

            modelBuilder.Entity("JLSDataModel.Models.ReferenceCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("LongLabel");

                    b.Property<string>("ShortLabel");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<bool?>("Validity");

                    b.HasKey("Id");

                    b.ToTable("ReferenceCategory");
                });

            modelBuilder.Entity("JLSDataModel.Models.ReferenceItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<int?>("Order");

                    b.Property<long?>("ParentId");

                    b.Property<long>("ReferenceCategoryId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<bool?>("Validity");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ReferenceCategoryId");

                    b.ToTable("ReferenceItem");
                });

            modelBuilder.Entity("JLSDataModel.Models.ReferenceLabel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("Label");

                    b.Property<string>("Lang");

                    b.Property<long>("ReferenceItemId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("ReferenceItemId");

                    b.ToTable("ReferenceLabel");
                });

            modelBuilder.Entity("JLSDataModel.Models.TokenModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClientId")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("ExpiryTime");

                    b.Property<DateTime>("LastModifiedDate");

                    b.Property<int>("UserId");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TokenModel");
                });

            modelBuilder.Entity("JLSDataModel.Models.User.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("EntrepriseName");

                    b.Property<long>("FacturationAdressId");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Siret");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<bool?>("Validity");

                    b.HasKey("Id");

                    b.HasIndex("FacturationAdressId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("JLSDataModel.Models.User.UserPreferenceCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long>("ReferenceCategoryId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<string>("UserId");

                    b.Property<int?>("UserId1");

                    b.HasKey("Id");

                    b.HasIndex("ReferenceCategoryId");

                    b.HasIndex("UserId1");

                    b.ToTable("UserPreferenceCategory");
                });

            modelBuilder.Entity("JLSDataModel.Models.User.UserShippingAdress", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<long>("ShippingAdressId");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ShippingAdressId");

                    b.HasIndex("UserId");

                    b.ToTable("UserShippingAdress");
                });

            modelBuilder.Entity("JLSDataModel.Models.User.UserToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<long?>("CreatedBy");

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<DateTime>("Expires");

                    b.Property<string>("Token");

                    b.Property<long?>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("JLSDataModel.Models.Audit.AuditData", b =>
                {
                    b.HasOne("JLSDataModel.Models.Audit.Audit", "Audit")
                        .WithMany()
                        .HasForeignKey("AuditId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderInfo", b =>
                {
                    b.HasOne("JLSDataModel.Models.Adress.Adress", "FacturationAdress")
                        .WithMany()
                        .HasForeignKey("FacturationAdressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.Adress.Adress", "ShippingAdress")
                        .WithMany()
                        .HasForeignKey("ShippingAdressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.ReferenceItem", "StatusReferenceItem")
                        .WithMany()
                        .HasForeignKey("StatusReferenceItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderInfoLog", b =>
                {
                    b.HasOne("JLSDataModel.Models.Order.OrderInfo", "OrderInfo")
                        .WithMany()
                        .HasForeignKey("OrderInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderInfoStatusLog", b =>
                {
                    b.HasOne("JLSDataModel.Models.Order.OrderInfo", "OrderInfo")
                        .WithMany()
                        .HasForeignKey("OrderInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Order.OrderProduct", b =>
                {
                    b.HasOne("JLSDataModel.Models.Order.OrderInfo", "OrderInfo")
                        .WithMany()
                        .HasForeignKey("OrderInfoId");

                    b.HasOne("JLSDataModel.Models.ReferenceItem", "Reference")
                        .WithMany()
                        .HasForeignKey("ReferenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.DiscountActivityProduct", b =>
                {
                    b.HasOne("JLSDataModel.Models.Product.DiscountActivity", "DiscountActivity")
                        .WithMany()
                        .HasForeignKey("DiscountActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.Product.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.Product", b =>
                {
                    b.HasOne("JLSDataModel.Models.ReferenceItem", "ReferenceItem")
                        .WithMany()
                        .HasForeignKey("ReferenceItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.ProductComment", b =>
                {
                    b.HasOne("JLSDataModel.Models.Product.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.Product.ProductPhotoPath", b =>
                {
                    b.HasOne("JLSDataModel.Models.Product.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.ReferenceItem", b =>
                {
                    b.HasOne("JLSDataModel.Models.ReferenceCategory", "ReferenceCategory")
                        .WithMany()
                        .HasForeignKey("ReferenceCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.ReferenceLabel", b =>
                {
                    b.HasOne("JLSDataModel.Models.ReferenceItem", "ReferenceItem")
                        .WithMany()
                        .HasForeignKey("ReferenceItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.TokenModel", b =>
                {
                    b.HasOne("JLSDataModel.Models.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.User.User", b =>
                {
                    b.HasOne("JLSDataModel.Models.Adress.Adress", "FacturationAdress")
                        .WithMany()
                        .HasForeignKey("FacturationAdressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.User.UserPreferenceCategory", b =>
                {
                    b.HasOne("JLSDataModel.Models.ReferenceCategory", "ReferenceCategory")
                        .WithMany()
                        .HasForeignKey("ReferenceCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("JLSDataModel.Models.User.UserShippingAdress", b =>
                {
                    b.HasOne("JLSDataModel.Models.Adress.Adress", "ShippingAdress")
                        .WithMany()
                        .HasForeignKey("ShippingAdressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JLSDataModel.Models.User.UserToken", b =>
                {
                    b.HasOne("JLSDataModel.Models.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("JLSDataModel.Models.User.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("JLSDataModel.Models.User.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JLSDataModel.Models.User.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("JLSDataModel.Models.User.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
