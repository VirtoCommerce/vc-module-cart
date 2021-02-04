using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public class InMemoryCartRepository
    {
        private IList<ShoppingCartEntity> ShoppingCartsStorage { get; set; }
        private IList<AddressEntity> AddressesStorage { get; set; }
        private IList<PaymentEntity> PaymentsStorage { get; set; }
        private IList<LineItemEntity> LineItemsStorage { get; set; }
        private IList<ShipmentEntity> ShipmentsStorage { get; set; }
        private IList<DiscountEntity> DiscountsStorage { get; set; }
        private IList<TaxDetailEntity> TaxDetailsStorage { get; set; }
        private IList<CouponEntity> CouponsStorage { get; set; }
        private IList<CartDynamicPropertyObjectValueEntity> DynamicPropertyObjectValuesStorage { get; set; }

        public IQueryable<ShoppingCartEntity> ShoppingCarts => ShoppingCartsStorage.AsQueryable();
        private IQueryable<AddressEntity> Addresses => AddressesStorage.AsQueryable();
        private IQueryable<PaymentEntity> Payments => PaymentsStorage.AsQueryable();
        private IQueryable<LineItemEntity> LineItems => LineItemsStorage.AsQueryable();
        private IQueryable<ShipmentEntity> Shipments => ShipmentsStorage.AsQueryable();
        private IQueryable<DiscountEntity> Discounts => DiscountsStorage.AsQueryable();
        private IQueryable<TaxDetailEntity> TaxDetails => TaxDetailsStorage.AsQueryable();
        private IQueryable<CouponEntity> Coupons => CouponsStorage.AsQueryable();
        private IQueryable<CartDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues => DynamicPropertyObjectValuesStorage.AsQueryable();

        public InMemoryCartRepository()
        {
            ShoppingCartsStorage = new List<ShoppingCartEntity>();
            AddressesStorage = new List<AddressEntity>();
            PaymentsStorage = new List<PaymentEntity>();
            LineItemsStorage = new List<LineItemEntity>();
            ShipmentsStorage = new List<ShipmentEntity>();
            DiscountsStorage = new List<DiscountEntity>();
            TaxDetailsStorage = new List<TaxDetailEntity>();
            CouponsStorage = new List<CouponEntity>();
            DynamicPropertyObjectValuesStorage = new List<CartDynamicPropertyObjectValueEntity>();
        }

        public void Add(ShoppingCartEntity cart)
        {
            if (cart != null)
            {
                ShoppingCartsStorage.Add(cart);

                if (!cart.Addresses.IsNullOrEmpty())
                {
                    AddressesStorage.AddRange(cart.Addresses);
                }

                if (!cart.Discounts.IsNullOrEmpty())
                {
                    DiscountsStorage.AddRange(cart.Discounts);
                }

                if (!cart.Items.IsNullOrEmpty())
                {
                    LineItemsStorage.AddRange(cart.Items);
                }

                if (!cart.Payments.IsNullOrEmpty())
                {
                    PaymentsStorage.AddRange(cart.Payments);
                }

                if (!cart.Shipments.IsNullOrEmpty())
                {
                    ShipmentsStorage.AddRange(cart.Shipments);
                }

                if (!cart.TaxDetails.IsNullOrEmpty())
                {
                    TaxDetailsStorage.AddRange(cart.TaxDetails);
                }

                if (!cart.Discounts.IsNullOrEmpty())
                {
                    DiscountsStorage.AddRange(cart.Discounts);
                }

                if (!cart.Coupons.IsNullOrEmpty())
                {
                    CouponsStorage.AddRange(cart.Coupons);
                }

                if (!cart.DynamicPropertyObjectValues.IsNullOrEmpty())
                {
                    DynamicPropertyObjectValuesStorage.AddRange(cart.DynamicPropertyObjectValues);
                }
            }
        }

        public async Task<ShoppingCartEntity[]> GetShoppingCartsByIdsAsync(string[] ids, string responseGroup = null)
        {
            var result = Array.Empty<ShoppingCartEntity>();

            if (!ids.IsNullOrEmpty())
            {
                result = ShoppingCartsStorage.Where(x => ids.Contains(x.Id)).ToArray();
            }

            return await Task.FromResult(result);
        }

        public void Remove(string cartId)
        {
            if (!string.IsNullOrEmpty(cartId))
            {
                var existingCart = ShoppingCartsStorage.FirstOrDefault(x => x.Id == cartId);
                if (existingCart != null)
                {
                    ShoppingCartsStorage.Remove(existingCart);
                }
            }
        }
    }
}
