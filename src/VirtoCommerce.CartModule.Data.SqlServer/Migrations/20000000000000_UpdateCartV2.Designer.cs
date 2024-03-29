// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VirtoCommerce.CartModule.Data.Repositories;

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    [DbContext(typeof(CartDbContext))]
    [Migration("20000000000000_UpdateCartV2")]
    partial class UpdateCartV2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.AddressEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("AddressType")
                    .HasMaxLength(32);

                b.Property<string>("City")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<string>("CountryCode")
                    .HasMaxLength(3);

                b.Property<string>("CountryName")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("Email")
                    .HasMaxLength(254);

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("Line1")
                    .HasMaxLength(2048);

                b.Property<string>("Line2")
                    .HasMaxLength(2048);

                b.Property<string>("Name")
                    .HasMaxLength(2048);

                b.Property<string>("Organization")
                    .HasMaxLength(64);

                b.Property<string>("PaymentId");

                b.Property<string>("Phone")
                    .HasMaxLength(64);

                b.Property<string>("PostalCode")
                    .HasMaxLength(64);

                b.Property<string>("RegionId")
                    .HasMaxLength(128);

                b.Property<string>("RegionName")
                    .HasMaxLength(128);

                b.Property<string>("ShipmentId");

                b.Property<string>("ShoppingCartId");

                b.HasKey("Id");

                b.HasIndex("PaymentId");

                b.HasIndex("ShipmentId");

                b.HasIndex("ShoppingCartId");

                b.ToTable("CartAddress");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.CouponEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("Code")
                    .HasMaxLength(64);

                b.Property<string>("ShoppingCartId")
                    .IsRequired();

                b.HasKey("Id");

                b.HasIndex("ShoppingCartId");

                b.ToTable("CartCoupon");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.DiscountEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CouponCode")
                    .HasMaxLength(64);

                b.Property<string>("Currency")
                    .IsRequired()
                    .HasMaxLength(3);

                b.Property<decimal>("DiscountAmount")
                    .HasColumnType("Money");

                b.Property<decimal>("DiscountAmountWithTax")
                    .HasColumnType("Money");

                b.Property<string>("LineItemId");

                b.Property<string>("PaymentId");

                b.Property<string>("PromotionDescription")
                    .HasMaxLength(1024);

                b.Property<string>("PromotionId")
                    .HasMaxLength(64);

                b.Property<string>("ShipmentId");

                b.Property<string>("ShoppingCartId");

                b.HasKey("Id");

                b.HasIndex("LineItemId");

                b.HasIndex("PaymentId");

                b.HasIndex("ShipmentId");

                b.HasIndex("ShoppingCartId");

                b.ToTable("CartDiscount");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.LineItemEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CatalogId")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("CategoryId")
                    .HasMaxLength(64);

                b.Property<string>("Comment")
                    .HasMaxLength(2048);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("Currency")
                    .IsRequired()
                    .HasMaxLength(3);

                b.Property<decimal>("DiscountAmount")
                    .HasColumnType("Money");

                b.Property<decimal>("DiscountAmountWithTax")
                    .HasColumnType("Money");

                b.Property<decimal>("Fee")
                    .HasColumnType("Money");

                b.Property<decimal>("FeeWithTax")
                    .HasColumnType("Money");

                b.Property<string>("FulfillmentLocationCode")
                    .HasMaxLength(64);

                b.Property<decimal?>("Height");

                b.Property<string>("ImageUrl")
                    .HasMaxLength(1028);

                b.Property<bool>("IsGift");

                b.Property<bool>("IsReadOnly");

                b.Property<bool>("IsReccuring");

                b.Property<string>("LanguageCode")
                    .HasMaxLength(16);

                b.Property<decimal?>("Length");

                b.Property<decimal>("ListPrice")
                    .HasColumnType("Money");

                b.Property<decimal>("ListPriceWithTax")
                    .HasColumnType("Money");

                b.Property<string>("MeasureUnit")
                    .HasMaxLength(32);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(256);

                b.Property<string>("PriceId")
                    .HasMaxLength(128);

                b.Property<string>("ProductId")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("ProductType")
                    .HasMaxLength(64);

                b.Property<int>("Quantity");

                b.Property<bool>("RequiredShipping");

                b.Property<decimal>("SalePrice")
                    .HasColumnType("Money");

                b.Property<decimal>("SalePriceWithTax")
                    .HasColumnType("Money");

                b.Property<string>("ShipmentMethodCode")
                    .HasMaxLength(64);

                b.Property<string>("ShoppingCartId")
                    .IsRequired();

                b.Property<string>("Sku")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<bool>("TaxIncluded");

                b.Property<decimal>("TaxPercentRate")
                    .HasColumnType("decimal(18,4)");

                b.Property<decimal>("TaxTotal")
                    .HasColumnType("Money");

                b.Property<string>("TaxType")
                    .HasMaxLength(64);

                b.Property<string>("ValidationType")
                    .HasMaxLength(64);

                b.Property<decimal?>("VolumetricWeight");

                b.Property<decimal?>("Weight");

                b.Property<string>("WeightUnit")
                    .HasMaxLength(32);

                b.Property<decimal?>("Width");

                b.HasKey("Id");

                b.HasIndex("ShoppingCartId");

                b.ToTable("CartLineItem");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.PaymentEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<decimal>("Amount")
                    .HasColumnType("Money");

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("Currency")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<decimal>("DiscountAmount")
                    .HasColumnType("Money");

                b.Property<decimal>("DiscountAmountWithTax")
                    .HasColumnType("Money");

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("PaymentGatewayCode")
                    .HasMaxLength(64);

                b.Property<decimal>("Price")
                    .HasColumnType("Money");

                b.Property<decimal>("PriceWithTax")
                    .HasColumnType("Money");

                b.Property<string>("Purpose")
                    .HasMaxLength(1024);

                b.Property<string>("ShoppingCartId")
                    .IsRequired();

                b.Property<decimal>("TaxPercentRate")
                    .HasColumnType("decimal(18,4)");

                b.Property<decimal>("TaxTotal")
                    .HasColumnType("Money");

                b.Property<string>("TaxType")
                    .HasMaxLength(64);

                b.Property<decimal>("Total")
                    .HasColumnType("Money");

                b.Property<decimal>("TotalWithTax")
                    .HasColumnType("Money");

                b.HasKey("Id");

                b.HasIndex("ShoppingCartId");

                b.ToTable("CartPayment");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.ShipmentEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("Currency")
                    .IsRequired()
                    .HasMaxLength(3);

                b.Property<decimal?>("DimensionHeight");

                b.Property<decimal?>("DimensionLength");

                b.Property<string>("DimensionUnit")
                    .HasMaxLength(16);

                b.Property<decimal?>("DimensionWidth");

                b.Property<decimal>("DiscountAmount")
                    .HasColumnType("Money");

                b.Property<decimal>("DiscountAmountWithTax")
                    .HasColumnType("Money");

                b.Property<decimal>("Fee")
                    .HasColumnType("Money");

                b.Property<decimal>("FeeWithTax")
                    .HasColumnType("Money");

                b.Property<string>("FulfillmentCenterId")
                    .HasMaxLength(64);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<decimal>("Price")
                    .HasColumnType("Money");

                b.Property<decimal>("PriceWithTax")
                    .HasColumnType("Money");

                b.Property<string>("ShipmentMethodCode")
                    .HasMaxLength(64);

                b.Property<string>("ShipmentMethodOption")
                    .HasMaxLength(64);

                b.Property<string>("ShoppingCartId")
                    .IsRequired();

                b.Property<bool>("TaxIncluded");

                b.Property<decimal>("TaxPercentRate")
                    .HasColumnType("decimal(18,4)");

                b.Property<decimal>("TaxTotal")
                    .HasColumnType("Money");

                b.Property<string>("TaxType")
                    .HasMaxLength(64);

                b.Property<decimal>("Total")
                    .HasColumnType("Money");

                b.Property<decimal>("TotalWithTax")
                    .HasColumnType("Money");

                b.Property<decimal?>("VolumetricWeight");

                b.Property<string>("WeightUnit")
                    .HasMaxLength(16);

                b.Property<decimal?>("WeightValue");

                b.HasKey("Id");

                b.HasIndex("ShoppingCartId");

                b.ToTable("CartShipment");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.ShipmentItemEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("BarCode")
                    .HasMaxLength(128);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("LineItemId")
                    .IsRequired();

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<int>("Quantity");

                b.Property<string>("ShipmentId")
                    .IsRequired();

                b.HasKey("Id");

                b.HasIndex("LineItemId");

                b.HasIndex("ShipmentId");

                b.ToTable("CartShipmentItem");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("ChannelId")
                    .HasMaxLength(64);

                b.Property<string>("Comment")
                    .HasMaxLength(2048);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("Currency")
                    .IsRequired()
                    .HasMaxLength(3);

                b.Property<string>("CustomerId")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("CustomerName")
                    .HasMaxLength(128);

                b.Property<decimal>("DiscountAmount")
                    .HasColumnType("Money");

                b.Property<decimal>("DiscountTotal")
                    .HasColumnType("Money");

                b.Property<decimal>("DiscountTotalWithTax")
                    .HasColumnType("Money");

                b.Property<decimal>("Fee")
                    .HasColumnType("Money");

                b.Property<decimal>("FeeWithTax")
                    .HasColumnType("Money");

                b.Property<decimal>("HandlingTotal")
                    .HasColumnType("Money");

                b.Property<decimal>("HandlingTotalWithTax")
                    .HasColumnType("Money");

                b.Property<bool>("IsAnonymous");

                b.Property<bool>("IsRecuring");

                b.Property<string>("LanguageCode")
                    .HasMaxLength(16);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .HasMaxLength(64);

                b.Property<string>("OrganizationId")
                    .HasMaxLength(64);

                b.Property<decimal>("PaymentTotal")
                    .HasColumnType("Money");

                b.Property<decimal>("PaymentTotalWithTax")
                    .HasColumnType("Money");

                b.Property<decimal>("ShippingTotal")
                    .HasColumnType("Money");

                b.Property<decimal>("ShippingTotalWithTax")
                    .HasColumnType("Money");

                b.Property<string>("Status")
                    .HasMaxLength(64);

                b.Property<string>("StoreId")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<decimal>("SubTotal")
                    .HasColumnType("Money");

                b.Property<decimal>("SubTotalWithTax")
                    .HasColumnType("Money");

                b.Property<bool>("TaxIncluded");

                b.Property<decimal>("TaxPercentRate")
                    .HasColumnType("decimal(18,4)");

                b.Property<decimal>("TaxTotal")
                    .HasColumnType("Money");

                b.Property<decimal>("Total")
                    .HasColumnType("Money");

                b.Property<string>("Type")
                    .HasMaxLength(64);

                b.Property<string>("ValidationType")
                    .HasMaxLength(64);

                b.HasKey("Id");

                b.ToTable("Cart");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.TaxDetailEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<decimal>("Amount")
                    .HasColumnType("Money");

                b.Property<string>("LineItemId");

                b.Property<string>("Name")
                    .HasMaxLength(1024);

                b.Property<string>("PaymentId");

                b.Property<decimal>("Rate");

                b.Property<string>("ShipmentId");

                b.Property<string>("ShoppingCartId");

                b.HasKey("Id");

                b.HasIndex("LineItemId");

                b.HasIndex("PaymentId");

                b.HasIndex("ShipmentId");

                b.HasIndex("ShoppingCartId");

                b.ToTable("CartTaxDetail");
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.AddressEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.PaymentEntity", "Payment")
                    .WithMany("Addresses")
                    .HasForeignKey("PaymentId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShipmentEntity", "Shipment")
                    .WithMany("Addresses")
                    .HasForeignKey("ShipmentId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", "ShoppingCart")
                    .WithMany("Addresses")
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.CouponEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", "ShoppingCart")
                    .WithMany("Coupons")
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.DiscountEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.LineItemEntity", "LineItem")
                    .WithMany("Discounts")
                    .HasForeignKey("LineItemId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.PaymentEntity", "Payment")
                    .WithMany("Discounts")
                    .HasForeignKey("PaymentId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShipmentEntity", "Shipment")
                    .WithMany("Discounts")
                    .HasForeignKey("ShipmentId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", "ShoppingCart")
                    .WithMany("Discounts")
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.LineItemEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", "ShoppingCart")
                    .WithMany("Items")
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.PaymentEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", "ShoppingCart")
                    .WithMany("Payments")
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.ShipmentEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", "ShoppingCart")
                    .WithMany("Shipments")
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.ShipmentItemEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.LineItemEntity", "LineItem")
                    .WithMany("ShipmentItems")
                    .HasForeignKey("LineItemId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShipmentEntity", "Shipment")
                    .WithMany("Items")
                    .HasForeignKey("ShipmentId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CartModule.Data.Model.TaxDetailEntity", b =>
            {
                b.HasOne("VirtoCommerce.CartModule.Data.Model.LineItemEntity", "LineItem")
                    .WithMany("TaxDetails")
                    .HasForeignKey("LineItemId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.PaymentEntity", "Payment")
                    .WithMany("TaxDetails")
                    .HasForeignKey("PaymentId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShipmentEntity", "Shipment")
                    .WithMany("TaxDetails")
                    .HasForeignKey("ShipmentId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CartModule.Data.Model.ShoppingCartEntity", "ShoppingCart")
                    .WithMany("TaxDetails")
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
#pragma warning restore 612, 618
        }
    }
}
