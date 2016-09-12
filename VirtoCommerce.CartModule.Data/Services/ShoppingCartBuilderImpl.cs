using System;
using System.Collections.Generic;
using System.Linq;
using CacheManager.Core;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Core.Common;
using System.Collections.Concurrent;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartBuilderImpl : IShoppingCartBuilder
    {
        private readonly IStoreService _storeService;
        private readonly IShoppingCartTaxEvaluator _taxEvaluator;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartSearchService _shoppingCartSearchService;
        private readonly IShoppingCartPromotionEvaluator _marketingPromoEvaluator;

        private readonly ICacheManager<object> _cacheManager;
        public const string CartCacheRegion = "CartRegion";

        private ShoppingCart _cart;

        private Store _store;

        [CLSCompliant(false)]
        public ShoppingCartBuilderImpl(IStoreService storeService, IShoppingCartTaxEvaluator taxEvaluator, IShoppingCartService shoppingShoppingCartService, IShoppingCartSearchService shoppingCartSearchService, IShoppingCartPromotionEvaluator marketingPromoEvaluator, ICacheManager<object> cacheManager)
        {
            _storeService = storeService;
            _taxEvaluator = taxEvaluator;
            _shoppingCartService = shoppingShoppingCartService;
            _shoppingCartSearchService = shoppingCartSearchService;
            _marketingPromoEvaluator = marketingPromoEvaluator;
            _cacheManager = cacheManager;
        }

        #region ICartBuilder Members

        public IShoppingCartBuilder TakeCart(ShoppingCart cart)
        {
            _cart = cart;
            return this;
        }

        public virtual IShoppingCartBuilder GetOrCreateNewTransientCart(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cart = _cacheManager.Get(GetCartCacheKey(storeId, cartName, customerId, currency), CartCacheRegion, () =>
            {
                var criteria = new ShoppingCartSearchCriteria
                {
                    CustomerId = string.IsNullOrEmpty(customerId) ? "anonymous" : customerId,
                    StoreId = storeId,
                    Name = cartName
                };
                var searchResult = _shoppingCartSearchService.Search(criteria);
                var cart = searchResult.Results.FirstOrDefault();
                if (cart == null)
                {
                    cart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();
                    cart.Name = cartName;
                    cart.Currency = currency;
                    cart.CustomerId = customerId;
                    cart.StoreId = storeId;
                    _shoppingCartService.SaveChanges(new[] { cart });
                    cart = _shoppingCartService.GetByIds(new[] { cart.Id }).FirstOrDefault();
                }
                return cart;
            });
            return this;
        }

        public virtual IShoppingCartBuilder AddItem(LineItem lineItem)
        {
            AddLineItem(lineItem);
            return this;
        }

        public virtual IShoppingCartBuilder ChangeItemQuantity(string id, int quantity)
        {
            var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
            if (lineItem != null)
            {
                InnerChangeItemQuantity(lineItem, quantity);
            }

            return this;
        }

        public virtual IShoppingCartBuilder RemoveItem(string id)
        {
            var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
            if (lineItem != null)
            {
                _cart.Items.Remove(lineItem);
            }

            return this;
        }

        public virtual IShoppingCartBuilder Clear()
        {
            _cart.Items.Clear();
            return this;
        }

        public virtual IShoppingCartBuilder AddCoupon(string couponCode)
        {
            _cart.Coupon = new Domain.Cart.Model.Coupon
            {
                Code = couponCode
            };

            return this;
        }

        public virtual IShoppingCartBuilder RemoveCoupon()
        {
            _cart.Coupon = null;

            return this;
        }

        public virtual IShoppingCartBuilder AddOrUpdateShipment(Shipment shipment)
        {
            Shipment existShipment = null;
            if (!shipment.IsTransient())
            {
                existShipment = _cart.Shipments.FirstOrDefault(s => s.Id == shipment.Id);
            }
            if (existShipment != null)
            {
                _cart.Shipments.Remove(existShipment);
            }
            shipment.Currency = _cart.Currency;
            _cart.Shipments.Add(shipment);

            if (!string.IsNullOrEmpty(shipment.ShipmentMethodCode))
            {
                var availableShippingRates = GetAvailableShippingRates();
                var shippingRate = availableShippingRates.FirstOrDefault(sm => (StringExtensions.EqualsInvariant(shipment.ShipmentMethodCode, sm.ShippingMethod.Code)) && (StringExtensions.EqualsInvariant(shipment.ShipmentMethodOption, sm.OptionName)));
                if (shippingRate == null)
                {
                    throw new Exception(string.Format("Unknown shipment method: {0} with option: {1}", shipment.ShipmentMethodCode, shipment.ShipmentMethodOption));
                }
                shipment.ShipmentMethodCode = shippingRate.ShippingMethod.Code;
                shipment.ShipmentMethodOption = shippingRate.OptionName;
                shipment.ShippingPrice = shippingRate.Rate;
                shipment.ShippingPriceWithTax = shippingRate.RateWithTax;
                shipment.DiscountTotal = shippingRate.DiscountAmount;
                shipment.DiscountTotalWithTax = shippingRate.DiscountAmountWithTax;
                shipment.TaxType = shippingRate.ShippingMethod.TaxType;
            }
            return this;
        }

        public virtual IShoppingCartBuilder RemoveShipment(string shipmentId)
        {
            var shipment = _cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                _cart.Shipments.Remove(shipment);
            }

            return this;
        }

        public virtual IShoppingCartBuilder AddOrUpdatePayment(Payment payment)
        {
            Payment existPayment = null;
            if (!payment.IsTransient())
            {
                existPayment = _cart.Payments.FirstOrDefault(s => s.Id == payment.Id);
            }

            if (existPayment != null)
            {
                _cart.Payments.Remove(existPayment);
            }
            _cart.Payments.Add(payment);

            if (!string.IsNullOrEmpty(payment.PaymentGatewayCode))
            {
                var availablePaymentMethods = GetAvailablePaymentMethods();
                var paymentMethod = availablePaymentMethods.FirstOrDefault(pm => string.Equals(pm.Code, payment.PaymentGatewayCode, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethod == null)
                {
                    throw new Exception("Unknown payment method " + payment.PaymentGatewayCode);
                }
            }
            return this;
        }

        public virtual IShoppingCartBuilder MergeWithCart(ShoppingCart cart)
        {
            foreach (var lineItem in cart.Items)
            {
                AddLineItem(lineItem);
            }
            _cart.Coupon = cart.Coupon;

            _cart.Shipments.Clear();
            _cart.Shipments = cart.Shipments;

            _cart.Payments.Clear();
            _cart.Payments = cart.Payments;

            _shoppingCartService.Delete(new[] { cart.Id });

            InvalidateCache();

            return this;
        }

        public virtual IShoppingCartBuilder RemoveCart()
        {
            _shoppingCartService.Delete(new string[] { _cart.Id });

            InvalidateCache();

            return this;
        }

        public virtual ICollection<ShippingRate> GetAvailableShippingRates()
        {
            // TODO: Remake with shipmentId
            var shippingEvaluationContext = new ShippingEvaluationContext(_cart);

            var activeAvailableShippingMethods = Store.ShippingMethods.Where(x => x.IsActive).ToList();

            var availableShippingRates = activeAvailableShippingMethods
                .SelectMany(x => x.CalculateRates(shippingEvaluationContext))
                .Where(x => x.ShippingMethod == null || x.ShippingMethod.IsActive)
                .ToArray();

            //Evaluate tax for shipping methods
            _taxEvaluator.EvaluateTaxes(this.Cart, availableShippingRates);
            //Evaluate promotions for shipping methods
            _marketingPromoEvaluator.EvaluatePromotions(this.Cart, availableShippingRates);
            return availableShippingRates;
        }

        public virtual ICollection<Domain.Payment.Model.PaymentMethod> GetAvailablePaymentMethods()
        {
            return Store.PaymentMethods.Where(x => x.IsActive).ToList();
        }

        public virtual IShoppingCartBuilder EvaluateTaxes()
        {
            _taxEvaluator.EvaluateTaxes(this.Cart);
            return this;
        }

        public virtual IShoppingCartBuilder EvaluatePromotions()
        {
            _marketingPromoEvaluator.EvaluatePromotions(this.Cart);
            return this;
        }

        public virtual void Save()
        {
            EvaluatePromotionsAndTaxes();
            //Invalidate cart in cache
            InvalidateCache();
            _shoppingCartService.SaveChanges(new[] { _cart });
        }

        public ShoppingCart Cart
        {
            get
            {
                return _cart;
            }
        }
        #endregion
        protected Store Store
        {
            get
            {
                if (_store == null)
                {
                    _store = _storeService.GetById(_cart.StoreId);
                }
                return _store;
            }
        }

        private void InvalidateCache()
        {
            _cacheManager.Remove(CartCaheKey, CartCacheRegion);
        }

        private string CartCaheKey
        {
            get
            {
                if (_cart == null)
                {
                    throw new Exception("Cart is not set");
                }
                return GetCartCacheKey(_cart.StoreId, _cart.Name, _cart.CustomerId, _cart.Currency);
            }
        }


        private void InnerChangeItemQuantity(LineItem lineItem, int quantity)
        {
            if (lineItem != null)
            {
                if (quantity > 0)
                {
                    lineItem.Quantity = quantity;
                }
                else
                {
                    _cart.Items.Remove(lineItem);
                }
            }
        }

        private void AddLineItem(LineItem lineItem)
        {
            var existingLineItem = _cart.Items.FirstOrDefault(li => li.ProductId == lineItem.ProductId);
            if (existingLineItem != null)
            {
                existingLineItem.Quantity += lineItem.Quantity;
            }
            else
            {
                lineItem.Id = null;
                _cart.Items.Add(lineItem);
            }
        }

        private void EvaluatePromotionsAndTaxes()
        {
            EvaluatePromotions();
            EvaluateTaxes();
        }

        private string GetCartCacheKey(string storeId, string cartName, string customerId, string currency)
        {
            return "CartBuilder:" + string.Join(storeId, cartName, customerId, currency);
        }
    }
}

