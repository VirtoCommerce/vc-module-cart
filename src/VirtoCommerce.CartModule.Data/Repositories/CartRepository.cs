using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return await GetShoppingCartsByIdsInternalAsync(ids, responseGroup, false);
        }

        public virtual async Task RemoveCartsAsync(string[] ids)
        {
            var carts = await GetShoppingCartsByIdsInternalAsync(ids, responseGroup: null, isDeleted: true);
            if (!carts.IsNullOrEmpty())
            {
                foreach (var cart in carts)
                {
                    // This extension is allow to get around breaking changes is introduced in EF Core 3.0 that leads to throw
                    // Database operation expected to affect 1 row(s) but actually affected 0 row(s) exception when trying to add the new children entities with manually set keys
                    // https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.0/breaking-changes#detectchanges-honors-store-generated-key-values
                    this.TrackModifiedAsAddedForNewChildEntities(cart);
                    Remove(cart);
                }
            }
        }

        public virtual async Task SoftRemoveCartsAsync(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                const string commandTemplate = @"
                    UPDATE Cart SET IsDeleted = 1 WHERE Id IN ({0})
                ";

                var cartsRemoveCmd = CreateCommand(commandTemplate, ids);
                await DbContext.ExecuteArrayAsync<string>(cartsRemoveCmd.Text, cartsRemoveCmd.Parameters.ToArray());
            }
        }

        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new SqlParameter($"@p{i}", v)).ToArray();
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>().ToList(),
            };
        }

        protected class Command
        {
            public string Text { get; set; }
            public IList<object> Parameters { get; set; } = new List<object>();
        }

        #endregion

        protected virtual async Task<ShoppingCartEntity[]> GetShoppingCartsByIdsInternalAsync(string[] ids, string responseGroup, bool isDeleted)
        {
            var result = Array.Empty<ShoppingCartEntity>();

            if (ids.IsNullOrEmpty())
            {
                return result;
            }

            result = ShoppingCarts.Where(x => ids.Contains(x.Id) && x.IsDeleted == isDeleted).ToArray();

            if (!result.Any())
            {
                return result;
            }

            ids = result.Select(x => x.Id).ToArray();

            await TaxDetails.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();
            await Discounts.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();
            await Addresses.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();
            await Coupons.Where(x => ids.Contains(x.ShoppingCartId)).LoadAsync();

            var cartResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CartResponseGroup.Full);

            await LoadPayments(ids, cartResponseGroup);
            await LoadLineItems(ids, cartResponseGroup);
            await LoadShipments(ids, cartResponseGroup);
            await LoadDynamicProperties(ids, cartResponseGroup);

            return result;
        }

        protected virtual async Task LoadPayments(string[] ids, CartResponseGroup cartResponseGroup)
        {
            if (cartResponseGroup.HasFlag(CartResponseGroup.WithPayments))
            {
                var payments = await Payments.Include(x => x.Addresses).Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                var paymentIds = payments.Select(x => x.Id).ToArray();

                if (paymentIds.Any())
                {
                    await TaxDetails.Where(x => paymentIds.Contains(x.PaymentId)).LoadAsync();
                    await Discounts.Where(x => paymentIds.Contains(x.PaymentId)).LoadAsync();

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                    {
                        var paymentTypeFullName = typeof(Payment).FullName;
                        await DynamicPropertyObjectValues
                            .Where(x => x.ObjectType == paymentTypeFullName && paymentIds.Contains(x.PaymentId))
                            .LoadAsync();
                    }
                }
            }
        }

        protected virtual async Task LoadLineItems(string[] ids, CartResponseGroup cartResponseGroup)
        {
            if (cartResponseGroup.HasFlag(CartResponseGroup.WithLineItems))
            {
                var lineItems = await LineItems.Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                var lineItemIds = lineItems.Select(x => x.Id).ToArray();

                if (lineItemIds.Any())
                {
                    await TaxDetails.Where(x => lineItemIds.Contains(x.LineItemId)).LoadAsync();
                    await Discounts.Where(x => lineItemIds.Contains(x.LineItemId)).LoadAsync();

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                    {
                        var lineItemTypeFullName = typeof(LineItem).FullName;
                        await DynamicPropertyObjectValues
                            .Where(x => x.ObjectType == lineItemTypeFullName && lineItemIds.Contains(x.LineItemId))
                            .LoadAsync();
                    }
                }
            }
        }

        protected virtual async Task LoadShipments(string[] ids, CartResponseGroup cartResponseGroup)
        {
            if (cartResponseGroup.HasFlag(CartResponseGroup.WithShipments))
            {
                var shipments = await Shipments.Include(x => x.Items).Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                var shipmentIds = shipments.Select(x => x.Id).ToArray();

                if (shipmentIds.Any())
                {
                    await TaxDetails.Where(x => shipmentIds.Contains(x.ShipmentId)).LoadAsync();
                    await Discounts.Where(x => shipmentIds.Contains(x.ShipmentId)).LoadAsync();
                    await Addresses.Where(x => shipmentIds.Contains(x.ShipmentId)).LoadAsync();

                    if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                    {
                        var shipmentTypeFullName = typeof(Shipment).FullName;
                        await DynamicPropertyObjectValues
                            .Where(x => x.ObjectType == shipmentTypeFullName && shipmentIds.Contains(x.ShipmentId))
                            .LoadAsync();
                    }
                }
            }
        }

        protected virtual async Task LoadDynamicProperties(string[] ids, CartResponseGroup cartResponseGroup)
        {
            if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
            {
                var shoppingCartTypeFullName = typeof(ShoppingCart).FullName;
                await DynamicPropertyObjectValues
                    .Where(x => x.ObjectType == shoppingCartTypeFullName && ids.Contains(x.ShoppingCartId))
                    .LoadAsync();
            }
        }
    }
}
