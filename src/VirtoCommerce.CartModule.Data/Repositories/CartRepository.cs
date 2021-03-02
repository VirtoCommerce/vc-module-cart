using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Extensions;
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
                            await TaxDetails.Where(x => lineItemIds.Contains(x.LineItemId)).LoadAsync();
                            await Discounts.Where(x => lineItemIds.Contains(x.LineItemId)).LoadAsync();
                            if (cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
                            {
                                var lineItemTypeFullName = typeof(LineItem).FullName;
                                await DynamicPropertyObjectValues.Where(x => x.ObjectType == lineItemTypeFullName && lineItemIds.Contains(x.LineItemId)).LoadAsync();
                            }
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
            if (!ids.IsNullOrEmpty())
            {
                // Forcibly remove CartDynamicPropertyObjectValue, CartDiscount, CartAddress, CartTaxDetail, CartShipmentItem
                // through plain query
                // Then remove carts
                // All other cart details will be removed using CASCADE DELETE db structure
                const string commandTemplate = @"
                    DELETE FROM CartDynamicPropertyObjectValue WHERE ShoppingCartId IN ({0})
                    DELETE CartDynamicPropertyObjectValue FROM CartDynamicPropertyObjectValue CartDynamicPropertyObjectValue INNER JOIN CartLineItem ON CartDynamicPropertyObjectValue.LineItemId=CartLineItem.id WHERE CartLineItem.ShoppingCartId IN ({0})
                    DELETE CartDynamicPropertyObjectValue FROM CartDynamicPropertyObjectValue CartDynamicPropertyObjectValue INNER JOIN CartShipment ON CartDynamicPropertyObjectValue.ShipmentId=CartShipment.id WHERE CartShipment.ShoppingCartId IN ({0})
                    DELETE CartDynamicPropertyObjectValue FROM CartDynamicPropertyObjectValue CartDynamicPropertyObjectValue INNER JOIN CartPayment ON CartDynamicPropertyObjectValue.PaymentId=CartPayment.id WHERE CartPayment.ShoppingCartId IN ({0})

                    DELETE FROM CartDiscount WHERE ShoppingCartId IN ({0})
                    DELETE CartDiscount FROM CartDiscount CartDiscount INNER JOIN CartLineItem ON CartDiscount.LineItemId=CartLineItem.id WHERE CartLineItem.ShoppingCartId IN ({0})
                    DELETE CartDiscount FROM CartDiscount CartDiscount INNER JOIN CartShipment ON CartDiscount.ShipmentId=CartShipment.id WHERE CartShipment.ShoppingCartId IN ({0})
                    DELETE CartDiscount FROM CartDiscount CartDiscount INNER JOIN CartPayment ON CartDiscount.PaymentId=CartPayment.id WHERE CartPayment.ShoppingCartId IN ({0})

                    DELETE FROM CartTaxDetail WHERE ShoppingCartId IN ({0})
                    DELETE CartTaxDetail FROM CartTaxDetail CartTaxDetail INNER JOIN CartLineItem ON CartTaxDetail.LineItemId=CartLineItem.id WHERE CartLineItem.ShoppingCartId IN ({0})
                    DELETE CartTaxDetail FROM CartTaxDetail CartTaxDetail INNER JOIN CartShipment ON CartTaxDetail.ShipmentId=CartShipment.id WHERE CartShipment.ShoppingCartId IN ({0})
                    DELETE CartTaxDetail FROM CartTaxDetail CartTaxDetail INNER JOIN CartPayment ON CartTaxDetail.PaymentId=CartPayment.id WHERE CartPayment.ShoppingCartId IN ({0})

                    DELETE CartShipmentItem FROM CartShipmentItem CartShipmentItem INNER JOIN CartLineItem ON CartShipmentItem.LineItemId=CartLineItem.id WHERE CartLineItem.ShoppingCartId IN ({0})
                    DELETE CartShipmentItem FROM CartShipmentItem CartShipmentItem INNER JOIN CartShipment ON CartShipmentItem.ShipmentId=CartShipment.id WHERE CartShipment.ShoppingCartId IN ({0})

                    DELETE FROM CartAddress WHERE ShoppingCartId IN ({0})
                    DELETE CartAddress FROM CartAddress CartAddress INNER JOIN CartPayment ON CartAddress.PaymentId=CartPayment.id WHERE CartPayment.ShoppingCartId IN ({0})
                    DELETE CartAddress FROM CartAddress CartAddress INNER JOIN CartShipment ON CartAddress.ShipmentId=CartShipment.id WHERE CartShipment.ShoppingCartId IN ({0})

                    DELETE FROM Cart WHERE Id IN ({0})
                ";

                var cartsRemoveCmd = CreateCommand(commandTemplate, ids);
                await DbContext.ExecuteArrayAsync<string>(cartsRemoveCmd.Text, cartsRemoveCmd.Parameters.ToArray());
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


        #endregion

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
    }
}
