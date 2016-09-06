using System;
using System.Collections.Generic;
using System.Linq;
using CacheManager.Core;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Data.Common;
using StringExtensions = VirtoCommerce.Platform.Core.Common.StringExtensions;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Order.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Platform.Core.DynamicProperties;
using Discount = VirtoCommerce.Domain.Cart.Model.Discount;
using LineItem = VirtoCommerce.Domain.Cart.Model.LineItem;
using Shipment = VirtoCommerce.Domain.Cart.Model.Shipment;
using VirtoCommerce.Platform.Core.Common;
using Omu.ValueInjecter;

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
        private const string _cartCacheRegion = "CartRegion";

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
            _cart = _cacheManager.Get(GetCartCacheKey(storeId, cartName, customerId), _cartCacheRegion, () =>
            {
                var cart = GetCurrentCart(customerId, storeId, cartName);
                if (cart == null)
                {
                    cart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();
                    cart.Name = cartName;
                    cart.Currency = currency;
                    cart.CustomerId = customerId;
                    cart.StoreId = storeId;
                    cart.Shipments = new List<Shipment>();
                    cart.Payments = new List<Payment>();
                    cart.Addresses = new List<Address>();
                    cart.Discounts = new List<Discount>();
                    cart.Items = new List<LineItem>();
                    cart.TaxDetails = new List<TaxDetail>();                
                }
                return cart;
            });

            return this;
        }   

        public virtual IShoppingCartBuilder AddItem(LineItem lineItem)
        {
            AddLineItem(lineItem);

            EvaluatePromotionsAndTaxes();
            return this;
        }

        public virtual IShoppingCartBuilder ChangeItemQuantity(string id, int quantity)
        {
            var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
            if (lineItem != null)
            {
                InnerChangeItemQuantity(lineItem, quantity);
                EvaluatePromotionsAndTaxes();
            }

            return this;
        }

        public virtual IShoppingCartBuilder RemoveItem(string id)
        {
            var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
            if (lineItem != null)
            {
                _cart.Items.Remove(lineItem);
                EvaluatePromotionsAndTaxes();
            }

            return this;
        }

        public virtual IShoppingCartBuilder Clear()
        {
            _cart.Items.Clear();

            EvaluatePromotionsAndTaxes();

            return this;
        }

        public virtual IShoppingCartBuilder AddCoupon(string couponCode)
        {
            _cart.Coupon = new Domain.Cart.Model.Coupon
            {
                Code = couponCode
            };

            EvaluatePromotionsAndTaxes();

            return this;
        }

        public virtual IShoppingCartBuilder RemoveCoupon()
        {
            _cart.Coupon = null;

            EvaluatePromotionsAndTaxes();

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
            EvaluatePromotionsAndTaxes();
            return this;
        }

        public virtual IShoppingCartBuilder RemoveShipment(string shipmentId)
        {
            var shipment = _cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                _cart.Shipments.Remove(shipment);
            }

            EvaluatePromotionsAndTaxes();

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
                payment.PaymentGatewayCode = paymentMethod.Code;
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

            EvaluatePromotionsAndTaxes();

            _shoppingCartService.Delete(new[] { cart.Id });

            _cacheManager.Remove(CartCaheKey, _cartCacheRegion);

            return this;
        }

        public virtual IShoppingCartBuilder RemoveCart()
        {
            _shoppingCartService.Delete(new string[] { _cart.Id });

            _cacheManager.Remove(CartCaheKey, _cartCacheRegion);

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
            //Invalidate cart in cache
            _cacheManager.Remove(CartCaheKey, _cartCacheRegion);
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

        private string CartCaheKey
        {
            get
            {
                if (_cart == null)
                {
                    throw new Exception("Cart is not set");
                }
                return GetCartCacheKey(_cart.StoreId, _cart.Name, _cart.CustomerId);
            }
        }


        private void InnerChangeItemQuantity(LineItem lineItem, int quantity)
        {
            if (lineItem != null)
            {
                //todo: tier price
                //var product = (await _catalogSearchService.GetProductsAsync(new[] { lineItem.ProductId }, ItemResponseGroup.ItemWithPrices)).FirstOrDefault();
                //if (product != null)
                //{
                //	lineItem.SalePrice = product.Price.GetTierPrice(quantity).Price;
                //}
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

        private ShoppingCart GetCurrentCart(string customerId, string storeId, string cartName)
        {
            var criteria = new ShoppingCartSearchCriteria
            {
                CustomerId = string.IsNullOrEmpty(customerId) ? "anonymous" : customerId,
                StoreId = storeId,
                Name = cartName
            };
            var searchResult = _shoppingCartSearchService.Search(criteria);
            var retVal = searchResult.Results.FirstOrDefault();
            return retVal;
        }

        private string GetCartCacheKey(string storeId, string cartName, string customerId)
        {
            return string.Format("Cart-{0}-{1}-{2}", storeId, cartName, customerId);
        }
    }
}

