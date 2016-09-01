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
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Order.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Data.Common;
using Coupon = VirtoCommerce.Domain.Cart.Model.Coupon;
using LineItem = VirtoCommerce.Domain.Cart.Model.LineItem;
using Shipment = VirtoCommerce.Domain.Cart.Model.Shipment;
using ShipmentItem = VirtoCommerce.Domain.Cart.Model.ShipmentItem;
using StringExtensions = VirtoCommerce.Platform.Core.Common.StringExtensions;

namespace VirtoCommerce.CartModule.Data.Builders
{
	public class CartBuilder : ICartBuilder
	{
		private readonly IStoreService _storeService;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IShoppingCartSearchService _shoppingCartSearchService;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly IDynamicPropertyService _dynamicPropertyService;
		private readonly IMarketingPromoEvaluator _marketingPromoEvaluator;

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
			_customerOrderService = customerOrderService;
			_dynamicPropertyService = dynamicPropertyService;
			_cacheManager = cacheManager;
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
						StoreId = storeId
					};
				}

				return cart;
			});

			return this;
		}

		public virtual ICartBuilder AddItem(AddItemModel addItemModel)
		{
			if (_cart.Items == null)
			{
				_cart.Items = new List<LineItem>();
			}

			var lineItem = new LineItem()
			{
				Currency = _cart.Currency,
				ProductId = addItemModel.ProductId,
				CatalogId = addItemModel.CatalogId,
				Sku = addItemModel.Sku,
				Name = addItemModel.Name,
				ImageUrl = addItemModel.ImageUrl,
				ListPrice = addItemModel.ListPrice,
				SalePrice = addItemModel.SalePrice,
				PlacedPrice = addItemModel.PlacedPrice,
				PlacedPriceWithTax = addItemModel.PlacedPrice,
				ExtendedPrice = addItemModel.ExtendedPrice,
				DiscountTotal = addItemModel.DiscountTotal,
				TaxTotal = addItemModel.TaxTotal,
				Quantity = addItemModel.Quantity,
				CreatedDate = DateTime.UtcNow
			};

			AddLineItem(lineItem);

			EvaluatePromotionsAndTaxes();
			CalculateLineItemPlacedPrice(lineItem);
			CalculateTotals();

			return this;
		}

		public virtual ICartBuilder ChangeItemQuantity(string id, int quantity)
		{
			var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
			if (lineItem != null)
			{
				InnerChangeItemQuantity(lineItem, quantity);
				EvaluatePromotionsAndTaxes();
				CalculateTotals();
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
				CalculateTotals();
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
			CalculateTotals();

			return this;
		}

		public virtual ICartBuilder RemoveItem(string id)
		{
			var lineItem = _cart.Items.FirstOrDefault(i => i.Id == id);
			if (lineItem != null)
			{
				_cart.Items.Remove(lineItem);
				EvaluatePromotionsAndTaxes();
				CalculateTotals();
			}

			return this;
		}

		public virtual ICartBuilder Clear()
		{
			_cart.Items.Clear();

			EvaluatePromotionsAndTaxes();
			CalculateTotals();

			return this;
		}

		public virtual ICartBuilder AddCoupon(string couponCode)
		{
			_cart.Coupon = new Coupon
			{
				Code = couponCode
			};

			EvaluatePromotionsAndTaxes();
			CalculateTotals();

			return this;
		}

		public virtual ICartBuilder RemoveCoupon()
		{
			_cart.Coupon = null;

			EvaluatePromotionsAndTaxes();
			CalculateTotals();

			return this;
		}

		public virtual ICartBuilder AddOrUpdateShipment(ShipmentUpdateModel updateModel)
		{
			var changedShipment = updateModel.ToShipmentModel(_cart.Currency);
			foreach (var updateItemModel in updateModel.Items)
			{
				var cartItem = _cart.Items.FirstOrDefault(i => i.Id == updateItemModel.LineItemId);
				if (cartItem != null)
				{
					var shipmentItem = cartItem.ToShipmentItem();
					shipmentItem.Quantity = updateItemModel.Quantity;
					changedShipment.Items.Add(shipmentItem);
				}
			}

			//Temporary support only one shipment in cart
			var shipment = _cart.Shipments.FirstOrDefault();
			if (!string.IsNullOrEmpty(changedShipment.Id))
			{
				shipment = _cart.Shipments.FirstOrDefault(s => s.Id == changedShipment.Id);
				if (shipment == null)
				{
					throw new Exception(string.Format("Shipment {0} not found", changedShipment.Id));
				}
			}

			if (shipment == null)
			{
				shipment = new Shipment()
				{
					Currency = _cart.Currency
				};
				_cart.Shipments.Add(shipment);
			}

			if (changedShipment.DeliveryAddress != null)
			{
				shipment.DeliveryAddress = changedShipment.DeliveryAddress;
			}

			//Update shipment items
			if (changedShipment.Items != null)
			{
				Action<EntryState, ShipmentItem, ShipmentItem> pathAction = (changeState, sourceItem, targetItem) =>
				{
					if (changeState == EntryState.Added)
					{
						var cartLineItem = _cart.Items.FirstOrDefault(i => i.Id == sourceItem.LineItem.Id);
						if (cartLineItem != null)
						{
							var newShipmentItem = cartLineItem.ToShipmentItem();
							newShipmentItem.Quantity = sourceItem.Quantity;
							shipment.Items.Add(newShipmentItem);
						}
					}
					else if (changeState == EntryState.Modified)
					{
						targetItem.Quantity = sourceItem.Quantity;
					}
					else if (changeState == EntryState.Deleted)
					{
						shipment.Items.Remove(sourceItem);
					}
				};

				var shipmentItemComparer = AnonymousComparer.Create((ShipmentItem x) => x.LineItem.Id);
				changedShipment.Items.CompareTo(shipment.Items, shipmentItemComparer, pathAction);
			}

			if (!string.IsNullOrEmpty(changedShipment.ShipmentMethodCode))
			{
				var availableShippingRates = GetAvailableShippingRates();
				var shippingRate = availableShippingRates.FirstOrDefault(sm => (StringExtensions.EqualsInvariant(changedShipment.ShipmentMethodCode, sm.ShippingMethod.Code)) && (StringExtensions.EqualsInvariant(changedShipment.ShipmentMethodOption, sm.OptionName)));
				if (shippingRate == null)
				{
					throw new Exception(string.Format("Unknown shipment method: {0} with option: {1}", changedShipment.ShipmentMethodCode, changedShipment.ShipmentMethodOption));
				}

				shipment.ShipmentMethodCode = shippingRate.ShippingMethod.Code;
				shipment.ShipmentMethodOption = shippingRate.OptionName;
				shipment.ShippingPrice = shippingRate.Rate;
				shipment.TaxType = shippingRate.ShippingMethod.TaxType;
			}

			EvaluatePromotionsAndTaxes();
			CalculateTotals();

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
			CalculateTotals();

			return this;
		}

		public virtual ICartBuilder AddOrUpdatePayment(PaymentUpdateModel updateModel)
		{
			Payment payment;
			if (!string.IsNullOrEmpty(updateModel.Id))
			{
				payment = _cart.Payments.FirstOrDefault(s => s.Id == updateModel.Id);
				if (payment == null)
				{
					throw new Exception(string.Format("Payment with {0} not found", updateModel.Id));
				}
			}
			else
			{
				payment = new Payment()
				{
					Currency = _cart.Currency
				};
				_cart.Payments.Add(payment);
			}

			if (updateModel.BillingAddress != null)
			{
				payment.BillingAddress = updateModel.BillingAddress;
			}

			if (!string.IsNullOrEmpty(updateModel.PaymentGatewayCode))
			{
				var availablePaymentMethods = GetAvailablePaymentMethods();
				var paymentMethod = availablePaymentMethods.FirstOrDefault(pm => string.Equals(pm.Code, updateModel.PaymentGatewayCode, StringComparison.InvariantCultureIgnoreCase));
				if (paymentMethod == null)
				{
					throw new Exception("Unknown payment method " + updateModel.PaymentGatewayCode);
				}
				payment.PaymentGatewayCode = paymentMethod.Code;
			}

			payment.OuterId = updateModel.OuterId;
			payment.Amount = _cart.Total;

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
			CalculateTotals();

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

		public virtual CustomerOrder ConvertCartToOrder()
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

		public virtual CreateOrderResult CreateOrder(CreateOrderModel createOrderModel)
		{
			var order = _customerOrderService.Create(ConvertCartToOrder());

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
					BankCardInfo = createOrderModel.BankCardInfo
				};
				result.ProcessPaymentResult = paymentMethod.ProcessPayment(context);

				_customerOrderService.Update(new[] {order});
			}

			return result;
		}

		//	public virtual async Task<ICartBuilder> FillFromQuoteRequest(QuoteRequest quoteRequest)
		//	{
		//		var productIds = quoteRequest.Items.Select(i => i.ProductId);
		//		var products = await _catalogSearchService.GetProductsAsync(productIds.ToArray(), ItemResponseGroup.ItemLarge);

		//		_cart.Items.Clear();
		//		foreach (var product in products)
		//		{
		//			var quoteItem = quoteRequest.Items.FirstOrDefault(i => i.ProductId == product.Id);
		//			if (quoteItem != null)
		//			{
		//				var lineItem = product.ToLineItem(_cart.Language, (int)quoteItem.SelectedTierPrice.Quantity);
		//				lineItem.ListPrice = quoteItem.ListPrice;
		//				lineItem.SalePrice = quoteItem.SelectedTierPrice.Price;
		//				lineItem.ValidationType = ValidationType.None;

		//				AddLineItem(lineItem);
		//			}
		//		}

		//		if (quoteRequest.RequestShippingQuote)
		//		{
		//			_cart.Shipments.Clear();
		//			var shipment = new Shipment(_cart.Currency);

		//			foreach (var item in _cart.Items)
		//			{
		//				shipment.Items.Add(item.ToShipmentItem());
		//			}

		//			if (quoteRequest.ShippingAddress != null)
		//			{
		//				shipment.DeliveryAddress = quoteRequest.ShippingAddress;
		//			}

		//			if (quoteRequest.ShipmentMethod != null)
		//			{
		//				var availableShippingMethods = await GetAvailableShippingMethodsAsync();
		//				if (availableShippingMethods != null)
		//				{
		//					var availableShippingMethod = availableShippingMethods.FirstOrDefault(sm => sm.ShipmentMethodCode == quoteRequest.ShipmentMethod.ShipmentMethodCode);
		//					if (availableShippingMethod != null)
		//					{
		//						shipment = quoteRequest.ShipmentMethod.ToShipmentModel(_cart.Currency);
		//					}
		//				}
		//			}

		//			_cart.Shipments.Add(shipment);
		//		}

		//		_cart.Payments.Clear();
		//		var payment = new Payment(_cart.Currency);

		//		if (quoteRequest.BillingAddress != null)
		//		{
		//			payment.BillingAddress = quoteRequest.BillingAddress;
		//		}

		//		payment.Amount = quoteRequest.Totals.GrandTotalInclTax;

		//		_cart.Payments.Add(payment);

		//		return this;
		//	}

		public virtual ICollection<Model.ShippingRate> GetAvailableShippingRates()
		{
			// TODO: Remake with shipmentId
			var shippingEvaluationContext = new ShippingEvaluationContext(_cart);

			var activeAvailableShippingMethods = Store.ShippingMethods.Where(x => x.IsActive).ToList();

			var availableShippingRates = activeAvailableShippingMethods
				.SelectMany(x => x.CalculateRates(shippingEvaluationContext))
				.Where(x => x.ShippingMethod == null || x.ShippingMethod.IsActive)
				.Select(x => x.ToDataModel()).ToArray();

			//Evaluate tax for shipping methods
			var activeTaxProvider = Store.TaxProviders.FirstOrDefault(x => x.IsActive);
			if (activeTaxProvider != null)
			{
				var taxEvaluationContext = _cart.ToTaxEvalContext();
				taxEvaluationContext.Lines.AddRange(availableShippingRates.Select(x => x.ToTaxLine()));
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
			var promotionEvaluationContext = _cart.ToPromotionEvaluationContext();
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
			var promotionEvaluationContext = _cart.ToPromotionEvaluationContext();
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
				var taxEvaluationContext = _cart.ToTaxEvalContext();
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

		private void CalculateLineItemPlacedPrice(LineItem lineItem)
		{
			var itemDiscount = lineItem.ListPrice - lineItem.SalePrice;
			var itemDiscountWithTax = lineItem.ListPriceWithTax - lineItem.SalePriceWithTax;
			if (lineItem.Discounts != null)
			{
				itemDiscount += lineItem.Discounts.Where(d => d != null).Sum(d => d.DiscountAmount);
				itemDiscountWithTax += lineItem.Discounts.Where(d => d != null).Sum(d => d.DiscountAmountWithTax);
			}

			lineItem.PlacedPrice = lineItem.ListPrice - itemDiscount;
			lineItem.PlacedPriceWithTax = lineItem.ListPriceWithTax - itemDiscountWithTax;
		}

		private void CalculateTotals()
		{
			Cart.SubTotal = Cart.Items?.Sum(i => i.ListPrice * i.Quantity) ?? 0;
			Cart.SubTotalWithTax = Cart.Items?.Sum(i => i.ListPriceWithTax * i.Quantity) ?? 0;

			Cart.ShippingTotal = Cart.Shipments?.Sum(s => s.ShippingPrice) ?? 0;
			Cart.ShippingTotalWithTax = Cart.Shipments?.Sum(s => s.ShippingPriceWithTax) ?? 0;

			var discountTotal = Cart.Discounts?.Sum(d => d.DiscountAmount) ?? 0;
			var itemDiscountTotal = Cart.Items?.Sum(i => GetItemDiscount(i) * i.Quantity) ?? 0;
			var shipmentDiscountTotal = Cart.Shipments?.Sum(s => GetShipmentDiscount(s)) ?? 0;
			Cart.DiscountTotal = discountTotal + itemDiscountTotal + shipmentDiscountTotal;

			var discountTotalWithTax = Cart.Discounts?.Sum(d => d.DiscountAmountWithTax) ?? 0;
			var itemDiscountTotalWithTax = Cart.Items?.Sum(i => GetItemDiscountWithTax(i) * i.Quantity) ?? 0;
			var shipmentDiscountTotalWithTax = Cart.Shipments?.Sum(s => GetShipmentDiscountWithTax(s)) ?? 0;
			Cart.DiscountTotalWithTax = discountTotalWithTax + itemDiscountTotalWithTax + shipmentDiscountTotalWithTax;

			Cart.Total = Cart.SubTotal + Cart.TaxTotal + Cart.ShippingTotal - Cart.DiscountTotal;
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

