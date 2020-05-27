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

                    await TaxDetails.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();
                    await Discounts.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();
                    await Addresses.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();
                    await Coupons.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();

                    var cartResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CartResponseGroup.Full);

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithPayments))
                    {
                        var payments = await Payments.Include(x => x.Addresses).Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                        var paymentIds = payments.Select(x => x.Id).ToArray();
                        if (paymentIds.Any())
                        {
                            await TaxDetails.Where(x => paymentIds.Contains(x.PaymentId)).LoadAsync();
                            await Discounts.Where(x => paymentIds.Contains(x.PaymentId)).LoadAsync();
                        }
                    }

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithLineItems))
                    {
                        var lineItems = await LineItems.Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                        var lineItemIds = lineItems.Select(x => x.Id).ToArray();

                        if (lineItemIds.Any())
                        {
                            var lineItemTypeFullName = typeof(LineItem).FullName;
                            await TaxDetails.Where(x => lineItemIds.Contains(x.LineItemId)).LoadAsync();
                            await Discounts.Where(x => lineItemIds.Contains(x.LineItemId)).LoadAsync();
                            await DynamicPropertyObjectValues.Where(x => x.ObjectType == lineItemTypeFullName && lineItemIds.Contains(x.LineItemId)).LoadAsync();
                        }
                    }

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithShipments))
                    {
                        var shipments = await Shipments.Include(x => x.Items).Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                        var shipmentIds = shipments.Select(x => x.Id).ToArray();

                        if (shipmentIds.Any())
                        {
                            await TaxDetails.Where(x => shipmentIds.Contains(x.ShipmentId)).LoadAsync();
                            await Discounts.Where(x => shipmentIds.Contains(x.ShipmentId)).LoadAsync();
                            await Addresses.Where(x => shipmentIds.Contains(x.ShipmentId)).LoadAsync();
                        }
                    }

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                    {
                        var shoppingCartTypeFullName = typeof(ShoppingCart).FullName;
                        // Function calls removed from LINQ Where because these can't be translated to SQL
                        await DynamicPropertyObjectValues.Where(x => x.ObjectType == shoppingCartTypeFullName && ids.Contains(x.ShoppingCartId)).LoadAsync();
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
