using System;
using System.Collections.Generic;
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
        private readonly ICartRawDatabaseCommand _rawDatabaseCommand;

        public CartRepository(CartDbContext dbContext, ICartRawDatabaseCommand rawDatabaseCommand)
            : base(dbContext)
        {
            _rawDatabaseCommand = rawDatabaseCommand;
        }

        public IQueryable<ShoppingCartEntity> ShoppingCarts => DbContext.Set<ShoppingCartEntity>();
        public IQueryable<AddressEntity> Addresses => DbContext.Set<AddressEntity>();
        public IQueryable<PaymentEntity> Payments => DbContext.Set<PaymentEntity>();
        public IQueryable<LineItemEntity> LineItems => DbContext.Set<LineItemEntity>();
        public IQueryable<ShipmentEntity> Shipments => DbContext.Set<ShipmentEntity>();
        protected IQueryable<DiscountEntity> Discounts => DbContext.Set<DiscountEntity>();
        protected IQueryable<TaxDetailEntity> TaxDetails => DbContext.Set<TaxDetailEntity>();
        protected IQueryable<CouponEntity> Coupons => DbContext.Set<CouponEntity>();
        protected IQueryable<ConfigurationItemEntity> ConfigurationItems => DbContext.Set<ConfigurationItemEntity>();
        protected IQueryable<CartDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues => DbContext.Set<CartDynamicPropertyObjectValueEntity>();
        protected IQueryable<ConfigurationItemFileEntity> ConfigurationItemFiles => DbContext.Set<ConfigurationItemFileEntity>();
        protected IQueryable<CartSharingSettingEntity> CartSharingSettings => DbContext.Set<CartSharingSettingEntity>();

        public virtual async Task<IList<ShoppingCartEntity>> GetShoppingCartsByIdsAsync(IList<string> ids, string responseGroup = null)
        {
            return await GetShoppingCartsByIdsInternalAsync(ids, responseGroup, false);
        }

        public virtual async Task RemoveCartsAsync(IList<string> ids)
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

        public virtual Task SoftRemoveCartsAsync(IList<string> ids)
        {
            return _rawDatabaseCommand.SoftRemove(DbContext, ids);
        }

        public Task<IList<ProductWishlistEntity>> FindWishlistsByProductsAsync(string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            return _rawDatabaseCommand.FindWishlistsByProductsAsync(DbContext, customerId, organizationId, storeId, productIds);
        }

        protected virtual async Task<IList<ShoppingCartEntity>> GetShoppingCartsByIdsInternalAsync(IList<string> ids, string responseGroup, bool isDeleted)
        {
            if (ids.IsNullOrEmpty())
            {
                return Array.Empty<ShoppingCartEntity>();
            }

            var carts = await ShoppingCarts
                .Include(x => x.TaxDetails)
                .Include(x => x.Discounts)
                .Include(x => x.Addresses)
                .Include(x => x.Coupons)
                .Include(x => x.SharingSettings)
                .Where(x => x.IsDeleted == isDeleted && ids.Contains(x.Id))
                .AsSingleQuery()
                .ToListAsync();

            if (carts.Any())
            {
                var cartIds = carts.Select(x => x.Id).ToArray();

                var cartResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CartResponseGroup.Full);

                await LoadPayments(cartIds, cartResponseGroup);
                await LoadLineItems(cartIds, cartResponseGroup);
                await LoadShipments(cartIds, cartResponseGroup);
                await LoadDynamicProperties(cartIds, cartResponseGroup);
            }

            return carts;
        }

        protected virtual async Task LoadPayments(IList<string> ids, CartResponseGroup cartResponseGroup)
        {
            if (cartResponseGroup.HasFlag(CartResponseGroup.WithPayments))
            {
                var paymentsQueryable = Payments
                    .Include(x => x.Addresses)
                    .Include(x => x.TaxDetails)
                    .Include(x => x.Discounts)
                    .AsQueryable();

                if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                {
                    var paymentTypeFullName = typeof(Payment).FullName;
                    paymentsQueryable = paymentsQueryable
                        .Include(x => x.DynamicPropertyObjectValues.Where(x => x.ObjectType == paymentTypeFullName));
                }

                await paymentsQueryable
                    .Where(x => ids.Contains(x.ShoppingCartId))
                    .AsSingleQuery()
                    .LoadAsync();
            }
        }

        protected virtual async Task LoadLineItems(IList<string> ids, CartResponseGroup cartResponseGroup)
        {
            if (cartResponseGroup.HasFlag(CartResponseGroup.WithLineItems))
            {
                var lineItemsQueryable = LineItems
                    .Include(x => x.TaxDetails)
                    .Include(x => x.Discounts)
                    .AsQueryable();

                if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                {
                    var lineItemTypeFullName = typeof(LineItem).FullName;
                    lineItemsQueryable = lineItemsQueryable
                        .Include(x => x.DynamicPropertyObjectValues.Where(x => x.ObjectType == lineItemTypeFullName));
                }

                var lineItems = await lineItemsQueryable
                    .Where(x => ids.Contains(x.ShoppingCartId))
                    .AsSingleQuery()
                    .ToListAsync();

                if (lineItems.Count > 0)
                {
                    var configurationItemIds = lineItems.Where(x => x.IsConfigured).Select(x => x.Id).ToList();
                    if (configurationItemIds.Count > 0)
                    {
                        await ConfigurationItems
                            .Include(x => x.Files)
                            .Where(x => configurationItemIds.Contains(x.LineItemId))
                            .AsSingleQuery()
                            .LoadAsync();
                    }
                }
            }
        }

        protected virtual async Task LoadShipments(IList<string> ids, CartResponseGroup cartResponseGroup)
        {
            if (cartResponseGroup.HasFlag(CartResponseGroup.WithShipments))
            {
                var shipmentsQueryable = Shipments
                    .Include(x => x.Items)
                    .Include(x => x.Addresses)
                    .Include(x => x.Discounts)
                    .Include(x => x.TaxDetails)
                    .AsQueryable();

                if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                {
                    var shipmentTypeFullName = typeof(Shipment).FullName;
                    shipmentsQueryable = shipmentsQueryable
                        .Include(x => x.DynamicPropertyObjectValues.Where(x => x.ObjectType == shipmentTypeFullName));
                }

                await shipmentsQueryable
                    .Where(x => ids.Contains(x.ShoppingCartId))
                    .AsSingleQuery()
                    .LoadAsync();
            }
        }

        protected virtual async Task LoadDynamicProperties(IList<string> ids, CartResponseGroup cartResponseGroup)
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
