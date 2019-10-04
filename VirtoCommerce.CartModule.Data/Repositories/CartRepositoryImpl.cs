using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public class CartRepositoryImpl : EFRepositoryBase, ICartRepository
    {
        public CartRepositoryImpl()
        {
        }

        public CartRepositoryImpl(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #region ShoppingCart
            modelBuilder.Entity<ShoppingCartEntity>().HasKey(x => x.Id)
                    .Property(x => x.Id);
            modelBuilder.Entity<ShoppingCartEntity>().Property(x => x.TaxPercentRate).HasPrecision(18, 4);

            modelBuilder.Entity<ShoppingCartEntity>().ToTable("Cart");
            #endregion

            #region LineItem
            modelBuilder.Entity<LineItemEntity>().HasKey(x => x.Id)
                    .Property(x => x.Id);
            modelBuilder.Entity<LineItemEntity>().Property(x => x.TaxPercentRate).HasPrecision(18, 4);

            modelBuilder.Entity<LineItemEntity>().HasRequired(x => x.ShoppingCart)
                                       .WithMany(x => x.Items)
                                       .HasForeignKey(x => x.ShoppingCartId).WillCascadeOnDelete(true);

            modelBuilder.Entity<LineItemEntity>().ToTable("CartLineItem");
            #endregion

            #region Shipment
            modelBuilder.Entity<ShipmentEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<ShipmentEntity>().Property(x => x.TaxPercentRate).HasPrecision(18, 4);
            modelBuilder.Entity<ShipmentEntity>().HasRequired(x => x.ShoppingCart)
                                           .WithMany(x => x.Shipments)
                                           .HasForeignKey(x => x.ShoppingCartId).WillCascadeOnDelete(true);


            modelBuilder.Entity<ShipmentEntity>().ToTable("CartShipment");
            #endregion

            #region ShipmentItemEntity
            modelBuilder.Entity<ShipmentItemEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);


            modelBuilder.Entity<ShipmentItemEntity>().HasRequired(x => x.LineItem)
                                       .WithMany(x => x.ShipmentItems)
                                       .HasForeignKey(x => x.LineItemId).WillCascadeOnDelete(true);

            modelBuilder.Entity<ShipmentItemEntity>().HasRequired(x => x.Shipment)
                                       .WithMany(x => x.Items)
                                       .HasForeignKey(x => x.ShipmentId).WillCascadeOnDelete(true);


            modelBuilder.Entity<ShipmentItemEntity>().ToTable("CartShipmentItem");
            #endregion

            #region Address
            modelBuilder.Entity<AddressEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);

            modelBuilder.Entity<AddressEntity>().HasOptional(x => x.ShoppingCart)
                                       .WithMany(x => x.Addresses)
                                       .HasForeignKey(x => x.ShoppingCartId).WillCascadeOnDelete(true);

            modelBuilder.Entity<AddressEntity>().HasOptional(x => x.Shipment)
                                       .WithMany(x => x.Addresses)
                                       .HasForeignKey(x => x.ShipmentId).WillCascadeOnDelete(true);

            modelBuilder.Entity<AddressEntity>().HasOptional(x => x.Payment)
                                       .WithMany(x => x.Addresses)
                                       .HasForeignKey(x => x.PaymentId).WillCascadeOnDelete(true);

            modelBuilder.Entity<AddressEntity>().ToTable("CartAddress");
            #endregion

            #region Payment
            modelBuilder.Entity<PaymentEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<PaymentEntity>().Property(x => x.TaxPercentRate).HasPrecision(18, 4);
            modelBuilder.Entity<PaymentEntity>().HasRequired(x => x.ShoppingCart)
                                       .WithMany(x => x.Payments)
                                       .HasForeignKey(x => x.ShoppingCartId).WillCascadeOnDelete(true);

            modelBuilder.Entity<PaymentEntity>().ToTable("CartPayment");
            #endregion

            #region TaxDetail
            modelBuilder.Entity<TaxDetailEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);


            modelBuilder.Entity<TaxDetailEntity>().HasOptional(x => x.ShoppingCart)
                                       .WithMany(x => x.TaxDetails)
                                       .HasForeignKey(x => x.ShoppingCartId).WillCascadeOnDelete(true);

            modelBuilder.Entity<TaxDetailEntity>().HasOptional(x => x.Shipment)
                                       .WithMany(x => x.TaxDetails)
                                       .HasForeignKey(x => x.ShipmentId).WillCascadeOnDelete(true);

            modelBuilder.Entity<TaxDetailEntity>().HasOptional(x => x.LineItem)
                                       .WithMany(x => x.TaxDetails)
                                       .HasForeignKey(x => x.LineItemId).WillCascadeOnDelete(true);

            modelBuilder.Entity<TaxDetailEntity>().HasOptional(x => x.Payment)
                                      .WithMany(x => x.TaxDetails)
                                      .HasForeignKey(x => x.PaymentId).WillCascadeOnDelete(true);


            modelBuilder.Entity<TaxDetailEntity>().ToTable("CartTaxDetail");
            #endregion

            #region Discount
            modelBuilder.Entity<DiscountEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);


            modelBuilder.Entity<DiscountEntity>().HasOptional(x => x.ShoppingCart)
                                       .WithMany(x => x.Discounts)
                                       .HasForeignKey(x => x.ShoppingCartId).WillCascadeOnDelete(true);

            modelBuilder.Entity<DiscountEntity>().HasOptional(x => x.Shipment)
                                       .WithMany(x => x.Discounts)
                                       .HasForeignKey(x => x.ShipmentId).WillCascadeOnDelete(true);

            modelBuilder.Entity<DiscountEntity>().HasOptional(x => x.LineItem)
                                       .WithMany(x => x.Discounts)
                                       .HasForeignKey(x => x.LineItemId).WillCascadeOnDelete(true);

            modelBuilder.Entity<DiscountEntity>().HasOptional(x => x.Payment)
                                     .WithMany(x => x.Discounts)
                                     .HasForeignKey(x => x.PaymentId).WillCascadeOnDelete(true);


            modelBuilder.Entity<DiscountEntity>().ToTable("CartDiscount");
            #endregion

            #region Coupon
            modelBuilder.Entity<CouponEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);


            modelBuilder.Entity<CouponEntity>().HasRequired(x => x.ShoppingCart)
                                       .WithMany(x => x.Coupons)
                                       .HasForeignKey(x => x.ShoppingCartId).WillCascadeOnDelete(true);


            modelBuilder.Entity<CouponEntity>().ToTable("CartCoupon");
            #endregion

            base.OnModelCreating(modelBuilder);
        }

        #region ICartRepository Members

        public IQueryable<ShoppingCartEntity> ShoppingCarts => GetAsQueryable<ShoppingCartEntity>();
        public IQueryable<AddressEntity> Addresses => GetAsQueryable<AddressEntity>();
        public IQueryable<PaymentEntity> Payments => GetAsQueryable<PaymentEntity>();
        public IQueryable<LineItemEntity> LineItems => GetAsQueryable<LineItemEntity>();
        public IQueryable<ShipmentEntity> Shipments => GetAsQueryable<ShipmentEntity>();
        protected IQueryable<DiscountEntity> Discounts => GetAsQueryable<DiscountEntity>();
        protected IQueryable<TaxDetailEntity> TaxDetails => GetAsQueryable<TaxDetailEntity>();
        protected IQueryable<CouponEntity> Coupons => GetAsQueryable<CouponEntity>();

        public virtual ShoppingCartEntity[] GetShoppingCartsByIds(string[] ids, string responseGroup = null)
        {
            // Array.Empty does not create empty array each time, all creations returns the same static object:
            // https://stackoverflow.com/a/33515349/5907312
            var result = Array.Empty<ShoppingCartEntity>();

            if (!ids.IsNullOrEmpty())
            {
                result = ShoppingCarts.Where(x => ids.Contains(x.Id)).ToArray();

                if (result.Any())
                {
                    ids = result.Select(x => x.Id).ToArray();

                    TaxDetails.Where(x => ids.Contains(x.ShoppingCartId)).Load();
                    Discounts.Where(x => ids.Contains(x.ShoppingCartId)).Load();
                    Addresses.Where(x => ids.Contains(x.ShoppingCartId)).Load();
                    Coupons.Where(x => ids.Contains(x.ShoppingCartId)).Load();

                    var payments = Payments.Include(x => x.Addresses).Where(x => ids.Contains(x.ShoppingCartId)).ToArray();
                    var paymentIds = payments.Select(x => x.Id).ToArray();

                    if (paymentIds.Any())
                    {
                        TaxDetails.Where(x => paymentIds.Contains(x.PaymentId)).Load();
                        Discounts.Where(x => paymentIds.Contains(x.PaymentId)).Load();
                    }

                    var lineItems = LineItems.Where(x => ids.Contains(x.ShoppingCartId)).ToArray();
                    var lineItemIds = lineItems.Select(x => x.Id).ToArray();

                    if (lineItemIds.Any())
                    {
                        TaxDetails.Where(x => lineItemIds.Contains(x.LineItemId)).Load();
                        Discounts.Where(x => lineItemIds.Contains(x.LineItemId)).Load();
                    }

                    var shipments = Shipments.Include(x => x.Items).Where(x => ids.Contains(x.ShoppingCartId)).ToArray();
                    var shipmentIds = shipments.Select(x => x.Id).ToArray();

                    if (shipmentIds.Any())
                    {
                        TaxDetails.Where(x => shipmentIds.Contains(x.ShipmentId)).Load();
                        Discounts.Where(x => shipmentIds.Contains(x.ShipmentId)).Load();
                        Addresses.Where(x => shipmentIds.Contains(x.ShipmentId)).Load();
                    }
                }
            }

            return result;
        }

        public virtual void RemoveCarts(string[] ids)
        {
            var carts = GetShoppingCartsByIds(ids);

            if (!carts.IsNullOrEmpty())
            {
                foreach (var cart in carts)
                {
                    Remove(cart);
                }
            }
        }

        public virtual void SoftRemoveCarts(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var carts = ShoppingCarts.Where(x => ids.Contains(x.Id));
                foreach (var cart in carts)
                {
                    cart.IsDeleted = true;
                }
            }
        }

        #endregion
    }
}
