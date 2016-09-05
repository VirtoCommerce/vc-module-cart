using System;
using System.Collections.Generic;
using System.Linq;
using CacheManager.Core;
using VirtoCommerce.CartModule.Data.Converters;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Common;
using StringExtensions = VirtoCommerce.Platform.Core.Common.StringExtensions;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Marketing.Model;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Tax.Model;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Order.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Platform.Core.DynamicProperties;
using Discount = VirtoCommerce.Domain.Cart.Model.Discount;
using LineItem = VirtoCommerce.Domain.Cart.Model.LineItem;
using Shipment = VirtoCommerce.Domain.Cart.Model.Shipment;

namespace VirtoCommerce.CartModule.Data.Builders
{
	public class CartBuilder : ICartBuilder
	{
		private readonly IStoreService _storeService;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IShoppingCartSearchService _shoppingCartSearchService;
		private readonly IMarketingPromoEvaluator _marketingPromoEvaluator;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly IDynamicPropertyService _dynamicPropertyService;

		private readonly ICacheManager<object> _cacheManager;
		private const string _cartCacheRegion = "CartRegion";

		private ShoppingCart _cart;

		private Store _store;

		[CLSCompliant(false)]
		public CartBuilder(IStoreService storeService, IShoppingCartService shoppingShoppingCartService, IShoppingCartSearchService shoppingCartSearchService, IMarketingPromoEvaluator marketingPromoEvaluator, ICacheManager<object> cacheManager, ICustomerOrderService customerOrderService, IDynamicPropertyService dynamicPropertyService)
		{
			_storeService = storeService;
			_shoppingCartService = shoppingShoppingCartService;
			_shoppingCartSearchService = shoppingCartSearchService;
			_marketingPromoEvaluator = marketingPromoEvaluator;
			_cacheManager = cacheManager;
			_customerOrderService = customerOrderService;
			_dynamicPropertyService = dynamicPropertyService;
		}

		#region ICartBuilder Members

		public ICartBuilder TakeCart(ShoppingCart cart)
		{
			_cart = cart;
			return this;
		}

		public virtual ICartBuilder GetOrCreateNewTransientCart(string storeId, string customerId, string cartName, string currency, string cultureName)
		{
			_cart = _cacheManager.Get(GetCartCacheKey(storeId, cartName, customerId), _cartCacheRegion, () =>
			{
				var cart = GetCurrentCart(customerId, storeId, cartName);
				if (cart == null)
				{
                    cart = new ShoppingCart()
                    {
                        Name = cartName,
                        Currency = currency,
                        CustomerId = customerId,
                        CustomerName = customerId,
                        StoreId = storeId,
                        Shipments = new List<Shipment>(),
                        Payments = new List<Payment>(),
                        Addresses = new List<Address>(),
                        Discounts = new List<Discount>(),
                        Items = new List<LineItem>(),
                        TaxDetails = new List<TaxDetail>()
                    };
                }

				return cart;
			});

			return this;
		}

        public virtual ICartBuilder AddProduct(CatalogProduct product, int quantity)
        {
            //TODO: Convert product -> line item and call AddItem(linbeitem)
            throw new NotImplementedException();
        }


        public virtual ICartBuilder AddItem(LineItem lineItem)
		{
			if (_cart.Items == null)
			{
				_cart.Items = new List<LineItem>();
			}
            		
			AddLineItem(lineItem);

			EvaluatePromotionsAndTaxes();
			return this;
		}

		public virtual ICartBuilder ChangeItemQuantity(string id, int quantity)
		{
			var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
			if (lineItem != null)
			{
				InnerChangeItemQuantity(lineItem, quantity);
				EvaluatePromotionsAndTaxes();
			}

			return this;
		}

		public virtual ICartBuilder ChangeItemQuantity(int lineItemIndex, int quantity)
		{
			var lineItem = _cart.Items.ElementAt(lineItemIndex);
			if (lineItem != null)
			{
				InnerChangeItemQuantity(lineItem, quantity);
				EvaluatePromotionsAndTaxes();
			}

			return this;
		}

		public virtual ICartBuilder ChangeItemsQuantities(int[] quantities)
		{
			for (var i = 0; i < quantities.Length; i++)
			{
				var lineItem = _cart.Items.ElementAt(i);
				if (lineItem != null && quantities[i] > 0)
				{
					InnerChangeItemQuantity(lineItem, quantities[i]);
				}
			}

			EvaluatePromotionsAndTaxes();

			return this;
		}

		public virtual ICartBuilder RemoveItem(string id)
		{
			var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
			if (lineItem != null)
			{
				_cart.Items.Remove(lineItem);
				EvaluatePromotionsAndTaxes();
			}

			return this;
		}

		public virtual ICartBuilder Clear()
		{
			_cart.Items.Clear();

			EvaluatePromotionsAndTaxes();

			return this;
		}

		public virtual ICartBuilder AddCoupon(string couponCode)
		{
			_cart.Coupon = new Domain.Cart.Model.Coupon
            {
				Code = couponCode
			};

			EvaluatePromotionsAndTaxes();

			return this;
		}

		public virtual ICartBuilder RemoveCoupon()
		{
			_cart.Coupon = null;

			EvaluatePromotionsAndTaxes();

			return this;
		}

		public virtual ICartBuilder AddOrUpdateShipment(Shipment sourceShipment)
		{  
            Shipment targetShipment = null;
            if (!sourceShipment.IsTransient())
            {
                targetShipment = _cart.Shipments.FirstOrDefault(s => s.Id == sourceShipment.Id);
            }
            if (targetShipment == null)
            {
                _cart.Shipments.Add(sourceShipment);
                targetShipment = sourceShipment;
            }

            if (sourceShipment.DeliveryAddress != null)
			{
                targetShipment.DeliveryAddress = sourceShipment.DeliveryAddress;
			}

            //if (sourceShipment.Items != null)
            //{
            //    sourceShipment.Items.Patch(targetShipment.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            //}

            if (!string.IsNullOrEmpty(sourceShipment.ShipmentMethodCode))
			{
				var availableShippingRates = GetAvailableShippingRates();
				var shippingRate = availableShippingRates.FirstOrDefault(sm => (StringExtensions.EqualsInvariant(sourceShipment.ShipmentMethodCode, sm.ShippingMethod.Code)) && (StringExtensions.EqualsInvariant(sourceShipment.ShipmentMethodOption, sm.OptionName)));
				if (shippingRate == null)
				{
					throw new Exception(string.Format("Unknown shipment method: {0} with option: {1}", sourceShipment.ShipmentMethodCode, sourceShipment.ShipmentMethodOption));
				}

                targetShipment.ShipmentMethodCode = shippingRate.ShippingMethod.Code;
                targetShipment.ShipmentMethodOption = shippingRate.OptionName;
                targetShipment.ShippingPrice = shippingRate.Rate;
                targetShipment.ShippingPriceWithTax = shippingRate.RateWithTax;
                targetShipment.DiscountTotal = shippingRate.DiscountAmount;
                targetShipment.DiscountTotalWithTax = shippingRate.DiscountAmountWithTax;
                targetShipment.TaxType = shippingRate.ShippingMethod.TaxType;
			}

            targetShipment.Currency = sourceShipment.Currency ?? _cart.Currency;
            targetShipment.Height = sourceShipment.Height;
            targetShipment.Length = sourceShipment.Length;
            targetShipment.MeasureUnit = sourceShipment.MeasureUnit;
            targetShipment.VolumetricWeight = sourceShipment.VolumetricWeight;
            targetShipment.WarehouseLocation = sourceShipment.WarehouseLocation;
            targetShipment.Weight = sourceShipment.Weight;
            targetShipment.WeightUnit = sourceShipment.WeightUnit;
            targetShipment.Width = sourceShipment.Width;


            EvaluatePromotionsAndTaxes();

			return this;
		}

		public virtual ICartBuilder RemoveShipment(string shipmentId)
		{
			var shipment = _cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
			if (shipment != null)
			{
				_cart.Shipments.Remove(shipment);
			}

			EvaluatePromotionsAndTaxes();

			return this;
		}

		public virtual ICartBuilder AddOrUpdatePayment(Payment sourcePayment)
		{  
            Payment targetPayment = null;
			if (!sourcePayment.IsTransient())
			{
                targetPayment = _cart.Payments.FirstOrDefault(s => s.Id == sourcePayment.Id);				
			}
            if (targetPayment == null)
            {
                _cart.Payments.Add(sourcePayment);
                targetPayment = sourcePayment;
            }
            if (sourcePayment.BillingAddress != null)
            {
                targetPayment.BillingAddress = sourcePayment.BillingAddress;
            }
            if (!string.IsNullOrEmpty(sourcePayment.PaymentGatewayCode))
			{
				var availablePaymentMethods = GetAvailablePaymentMethods();
				var paymentMethod = availablePaymentMethods.FirstOrDefault(pm => string.Equals(pm.Code, sourcePayment.PaymentGatewayCode, StringComparison.InvariantCultureIgnoreCase));
				if (paymentMethod == null)
				{
					throw new Exception("Unknown payment method " + sourcePayment.PaymentGatewayCode);
				}
                targetPayment.PaymentGatewayCode = paymentMethod.Code;
			}
            targetPayment.Currency = sourcePayment.Currency ?? _cart.Currency;
            targetPayment.OuterId = sourcePayment.OuterId;
            targetPayment.Amount = sourcePayment.Amount > 0 ? sourcePayment.Amount : _cart.Total;
			return this;
		}

		public virtual ICartBuilder MergeWithCart(ShoppingCart cart)
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

		public virtual ICartBuilder RemoveCart()
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
			var activeTaxProvider = Store.TaxProviders.FirstOrDefault(x => x.IsActive);
			if (activeTaxProvider != null)
			{
				var taxEvaluationContext = ToTaxEvalContext();
                foreach(var shippingRate in availableShippingRates)
                {
                    var taxLine = new TaxLine
                    {
                        Id = string.Join("&", shippingRate.ShippingMethod.Code, shippingRate.OptionName),
                        Code = string.Join("&", shippingRate.ShippingMethod.Code, shippingRate.OptionName),
                        Name = string.Join("&", shippingRate.ShippingMethod.Name, shippingRate.OptionDescription),
                        TaxType = shippingRate.ShippingMethod.TaxType,
                        Amount = shippingRate.Rate
                    };
                    taxEvaluationContext.Lines.Add(taxLine);
                }
				taxEvaluationContext.Store = Store;

				var taxRates = activeTaxProvider.CalculateRates(taxEvaluationContext);
				if (taxRates != null)
				{
					foreach (var shippingRate in availableShippingRates)
					{
						shippingRate.ApplyTaxRates(taxRates);
					}
				}
			}

			//Evaluate promotions for shipping methods
			var promotionEvaluationContext = ToPromotionEvaluationContext();
			var promotionResult = _marketingPromoEvaluator.EvaluatePromotion(promotionEvaluationContext);
			if (promotionResult.Rewards != null)
			{
				foreach (var availableShippingRate in availableShippingRates)
				{
					availableShippingRate.ApplyRewards(promotionResult.Rewards);
				}
			}

			return availableShippingRates;
		}

		public virtual ICollection<Domain.Payment.Model.PaymentMethod> GetAvailablePaymentMethods()
		{
			return Store.PaymentMethods.Where(x => x.IsActive).ToList();
		}

		public virtual ICartBuilder EvaluatePromotions()
		{
			var promotionEvaluationContext = ToPromotionEvaluationContext();
			var promotionResult = _marketingPromoEvaluator.EvaluatePromotion(promotionEvaluationContext);
			if (promotionResult.Rewards != null)
			{
				_cart.ApplyRewards(promotionResult.Rewards);
			}

			return this;
		}

		public ICartBuilder EvaluateTax()
		{
			var activeTaxProvider = Store.TaxProviders.FirstOrDefault(x => x.IsActive);
			if (activeTaxProvider != null)
			{
				var taxEvaluationContext = ToTaxEvalContext();
				var taxRates = activeTaxProvider.CalculateRates(taxEvaluationContext);
				_cart.ApplyTaxeRates(taxRates);
			}

			return this;
		}

		public virtual void Save()
		{
			//Invalidate cart in cache
			_cacheManager.Remove(CartCaheKey, _cartCacheRegion);

			if (_cart.IsTransient())
			{
				_cart = _shoppingCartService.Create(_cart);
			}
			else
			{
				_shoppingCartService.Update(new ShoppingCart[] { _cart });
			}
		}

		public virtual CreateOrderResult CreateOrder(BankCardInfo bankCardInfo)
		{
			var order = ConvertCartToOrder();

			_customerOrderService.SaveChanges(new CustomerOrder[] {order});

			RemoveCart();

			var result = new CreateOrderResult()
			{
				Order = order
			};

			var incomingPayment = order.InPayments?.FirstOrDefault();
			if (incomingPayment != null)
			{
				var paymentMethods = GetAvailablePaymentMethods();
				var paymentMethod = paymentMethods.FirstOrDefault(x => x.Code == incomingPayment.GatewayCode);
				if (paymentMethod == null)
				{
					throw new Exception("An appropriate paymentMethod is not found.");
				}

				result.PaymentMethodType = paymentMethod.PaymentMethodType;

				var context = new ProcessPaymentEvaluationContext
				{
					Order = order,
					Payment = incomingPayment,
					Store = Store,
					BankCardInfo = bankCardInfo
				};
				result.ProcessPaymentResult = paymentMethod.ProcessPayment(context);

				_customerOrderService.SaveChanges(new[] { order });
			}

			return result;
		}

		public ShoppingCart Cart
		{
			get
			{
				return _cart;
			}
		}

		public Store Store
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

		#endregion

        protected virtual TaxEvaluationContext ToTaxEvalContext()
        {
            var retVal = new TaxEvaluationContext
            {
                Id = this.Cart.Id,
                Code = this.Cart.Name,
                Currency = this.Cart.Currency,
                Type = "Cart",
                Lines = new List<TaxLine>()
            };

            if (this.Cart.Items != null)
            {
                foreach (var lineItem in this.Cart.Items)
                {
                    var extendedTaxLine = new TaxLine
                    {
                        Id = lineItem.Id + "&extended",
                        Code = lineItem.Sku,
                        Name = lineItem.Name,
                        TaxType = lineItem.TaxType,
                        Amount = lineItem.ExtendedPrice
                    };
                    retVal.Lines.Add(extendedTaxLine);

                    var listTaxLine = new TaxLine
                    {
                        Id = lineItem.Id + "&list",
                        Code = lineItem.Sku,
                        Name = lineItem.Name,
                        TaxType = lineItem.TaxType,
                        Amount = lineItem.ListPrice
                    };
                    retVal.Lines.Add(listTaxLine);

                    if (lineItem.ListPrice != lineItem.SalePrice)
                    {
                        var saleTaxLine = new TaxLine
                        {
                            Id = lineItem.Id + "&sale",
                            Code = lineItem.Sku,
                            Name = lineItem.Name,
                            TaxType = lineItem.TaxType,
                            Amount = lineItem.SalePrice
                        };
                        retVal.Lines.Add(saleTaxLine);
                    }
                }
            }

            if (this.Cart.Shipments != null)
            {
                foreach (var shipment in this.Cart.Shipments)
                {
                    var totalTaxLine = new TaxLine
                    {
                        Id = shipment.Id + "&total",
                        Code = shipment.ShipmentMethodCode,
                        Name = shipment.ShipmentMethodCode,
                        TaxType = shipment.TaxType,
                        Amount = shipment.Total
                    };
                    retVal.Lines.Add(totalTaxLine);
                    var priceTaxLine = new TaxLine
                    {
                        Id = shipment.Id + "&price",
                        Code = shipment.ShipmentMethodCode,
                        Name = shipment.ShipmentMethodCode,
                        TaxType = shipment.TaxType,
                        Amount = shipment.ShippingPrice
                    };
                    retVal.Lines.Add(priceTaxLine);

                    if (shipment.DeliveryAddress != null)
                    {
                        retVal.Address = shipment.DeliveryAddress;
                        retVal.Address.AddressType = shipment.DeliveryAddress.AddressType;
                    }

                    retVal.Customer = new Contact
                    {
                        Id = this.Cart.CustomerId,
                        Name = this.Cart.CustomerName
                    };
                }
            }
            return retVal;
        }

        protected virtual PromotionEvaluationContext ToPromotionEvaluationContext()
        {
            List<ProductPromoEntry> promotionItems = new List<ProductPromoEntry>();

            if (this.Cart.Items != null)
            {
                foreach(var lineItem in this.Cart.Items)
                {
                    var promoItem = new ProductPromoEntry();

                    promoItem.InjectFrom(lineItem);

                    promoItem.Discount = lineItem.DiscountTotal;
                    promoItem.Price = lineItem.PlacedPrice;
                    promoItem.Quantity = lineItem.Quantity;
                    promoItem.Variations = null; // TODO

                    promotionItems.Add(promoItem);
                }               
            }

            var retVal = new PromotionEvaluationContext
            {
                CartPromoEntries = promotionItems,
                CartTotal = this.Cart.Total,
                Coupon = this.Cart.Coupon?.Code,
                Currency = this.Cart.Currency,
                CustomerId = this.Cart.CustomerId,
                //todo: IsRegisteredUser = cart.Customer.IsRegisteredUser,
                Language = this.Cart.LanguageCode,
                PromoEntries = promotionItems,
                StoreId = this.Cart.StoreId
            };

            return retVal;
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

		private CustomerOrder ConvertCartToOrder()
		{
			var customerOrder = Cart.ToCustomerOrder();

			// Copy dynamic properties
			_dynamicPropertyService.LoadDynamicPropertyValues(customerOrder);
			foreach (var cartItem in Cart.Items)
			{
				var orderItem = customerOrder.Items.FirstOrDefault(x => x.ProductId == cartItem.ProductId && x.Quantity == cartItem.Quantity);
				orderItem?.CopyPropertyValuesFrom(cartItem);
			}

			return customerOrder;
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
			EvaluateTax();
		}

		private ShoppingCart GetCurrentCart(string customerId, string storeId, string cartName)
		{
			var criteria = new Domain.Cart.Model.SearchCriteria
			{
				CustomerId = string.IsNullOrEmpty(customerId) ? "anonymous" : customerId,
				StoreId = storeId,
				Keyword = cartName
			};

			var searchResult = _shoppingCartSearchService.Search(criteria);

			var retVal = searchResult.ShopingCarts.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals("default", StringComparison.OrdinalIgnoreCase))
						 ?? searchResult.ShopingCarts.FirstOrDefault();

			if (retVal != null)
			{
				retVal = _shoppingCartService.GetById(retVal.Id);
			}

			return retVal;
		}

		private string GetCartCacheKey(string storeId, string cartName, string customerId)
		{
			return string.Format("Cart-{0}-{1}-{2}", storeId, cartName, customerId);
		}
	

		private static decimal GetShipmentDiscount(Shipment shipment)
		{
			decimal retVal = 0;
			if (shipment.Discounts != null)
			{
				retVal = shipment.Discounts.Sum(s => s.DiscountAmount);
			}
			return retVal;
		}

		private static decimal GetShipmentDiscountWithTax(Shipment shipment)
		{
			decimal retVal = 0;
			if (shipment.Discounts != null)
			{
				retVal = shipment.Discounts.Sum(s => s.DiscountAmountWithTax);
			}
			return retVal;
		}

		private static decimal GetItemDiscount(LineItem lineItem)
		{
			var retVal = lineItem.ListPrice - lineItem.SalePrice;
			if (!lineItem.Discounts.IsNullOrEmpty())
			{
				retVal += lineItem.Discounts.Where(d => d != null).Sum(d => d.DiscountAmount);
			}

			return retVal;
		}

		private static decimal GetItemDiscountWithTax(LineItem lineItem)
		{
			var retVal = lineItem.ListPriceWithTax - lineItem.SalePriceWithTax;
			if (!lineItem.Discounts.IsNullOrEmpty())
			{
				retVal += lineItem.Discounts.Where(d => d != null).Sum(d => d.DiscountAmountWithTax);
			}
			return retVal;
		}
	}
}

