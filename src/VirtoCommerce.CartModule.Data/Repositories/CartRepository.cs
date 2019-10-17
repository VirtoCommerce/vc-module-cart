using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public class CartRepository : DbContextRepositoryBase<CartDbContext>, ICartRepository
    {
        public CartRepository(CartDbContext dbContext) : base(dbContext)
        {
        }

        #region ICartRepository Members

        public IQueryable<ShoppingCartEntity> ShoppingCarts => DbContext.Set<ShoppingCartEntity>();
        public IQueryable<AddressEntity> Addresses => DbContext.Set<AddressEntity>();
        public IQueryable<PaymentEntity> Payments => DbContext.Set<PaymentEntity>();
        public IQueryable<LineItemEntity> LineItems => DbContext.Set<LineItemEntity>();
        public IQueryable<ShipmentEntity> Shipments => DbContext.Set<ShipmentEntity>();
        protected IQueryable<DiscountEntity> Discounts => DbContext.Set<DiscountEntity>();
        protected IQueryable<TaxDetailEntity> TaxDetails => DbContext.Set<TaxDetailEntity>();
        protected IQueryable<CouponEntity> Coupons => DbContext.Set<CouponEntity>();
        protected IQueryable<CartDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues => DbContext.Set<CartDynamicPropertyObjectValueEntity>();

        public virtual async Task<ShoppingCartEntity[]> GetShoppingCartsByIdsAsync(string[] ids, string responseGroup = null)
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

                    var cartResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CartResponseGroup.Full);

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithPayments))
                    {
                        var payments = await Payments.Include(x => x.Addresses).Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                        var paymentIds = payments.Select(x => x.Id).ToArray();
                        if (paymentIds.Any())
                        {
                            TaxDetails.Where(x => paymentIds.Contains(x.PaymentId)).Load();
                            Discounts.Where(x => paymentIds.Contains(x.PaymentId)).Load();
                        }
                    }

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithLineItems))
                    {
                        var lineItems = LineItems.Where(x => ids.Contains(x.ShoppingCartId)).ToArray();
                        var lineItemIds = lineItems.Select(x => x.Id).ToArray();

                        if (lineItemIds.Any())
                        {
                            TaxDetails.Where(x => lineItemIds.Contains(x.LineItemId)).Load();
                            Discounts.Where(x => lineItemIds.Contains(x.LineItemId)).Load();
                        }
                    }

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithShipments))
                    {
                        var shipments = Shipments.Include(x => x.Items).Where(x => ids.Contains(x.ShoppingCartId)).ToArray();
                        var shipmentIds = shipments.Select(x => x.Id).ToArray();

                        if (shipmentIds.Any())
                        {
                            TaxDetails.Where(x => shipmentIds.Contains(x.ShipmentId)).Load();
                            Discounts.Where(x => shipmentIds.Contains(x.ShipmentId)).Load();
                            Addresses.Where(x => shipmentIds.Contains(x.ShipmentId)).Load();
                        }
                    }

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                    {
                        DynamicPropertyObjectValues.Where(x => x.ObjectType.EqualsInvariant(typeof(ShoppingCart).FullName) && ids.Contains(x.ShoppingCartId)).Load();
                    }
                }
            }
            return result;
        }

        public virtual async Task RemoveCartsAsync(string[] ids)
        {
            var carts = await GetShoppingCartsByIdsAsync(ids);
            if (!carts.IsNullOrEmpty())
            {
                foreach (var cart in carts)
                {
                    Remove(cart);
                }
            }
        }

        public virtual async Task SoftRemoveCartsAsync(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var carts = await ShoppingCarts.Where(x => ids.Contains(x.Id)).ToListAsync();
                foreach (var cart in carts)
                {
                    cart.IsDeleted = true;
                }
            }
        }

        #endregion
    }
}
