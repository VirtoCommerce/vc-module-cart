using System.Collections.Generic;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Payment.Model;

namespace VirtoCommerce.CartModule.Data.Builders
{
    /// <summary>
    /// Represent abstraction for working with customer shopping cart
    /// </summary>
    public interface ICartBuilder
	{
		/// <summary>
		///  Capture passed cart and all next changes will be implemented on it
		/// </summary>
		/// <param name="cart"></param>
		/// <returns></returns>
		ICartBuilder TakeCart(ShoppingCart cart);

		/// <summary>
		/// Load or created new cart for current user and capture it
		/// </summary>
		/// <returns></returns>
		ICartBuilder GetOrCreateNewTransientCart(string storeId, string customerId, string cartName, string currency, string cultureName);

		/// <summary>
		/// Add new lineitem  to cart
		/// </summary>
		/// <param name="addItemModel"></param>
		/// <returns></returns>
		ICartBuilder AddItem(LineItem lineItem);

        /// <summary>
		/// Add new product to cart
		/// </summary>
		/// <param name="addItemModel"></param>
		/// <returns></returns>
		ICartBuilder AddProduct(CatalogProduct product, int quantity);

        /// <summary>
        /// Change cart item qty by product index
        /// </summary>
        /// <param name="lineItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        ICartBuilder ChangeItemQuantity(string lineItemId, int quantity);

		/// <summary>
		/// Change cart item qty by item id
		/// </summary>
		/// <param name="lineItemIndex"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		ICartBuilder ChangeItemQuantity(int lineItemIndex, int quantity);

		ICartBuilder ChangeItemsQuantities(int[] quantities);

		/// <summary>
		/// Remove item from cart by id
		/// </summary>
		/// <param name="lineItemId"></param>
		/// <returns></returns>
		ICartBuilder RemoveItem(string lineItemId);

		/// <summary>
		/// Apply marketing coupon to captured cart
		/// </summary>
		/// <param name="couponCode"></param>
		/// <returns></returns>
		ICartBuilder AddCoupon(string couponCode);

		/// <summary>
		/// remove exist coupon from cart
		/// </summary>
		/// <returns></returns>
		ICartBuilder RemoveCoupon();

		/// <summary>
		/// Clear cart remove all items and shipments and payments
		/// </summary>
		/// <returns></returns>
		ICartBuilder Clear();

		/// <summary>
		/// Add or update shipment to cart
		/// </summary>
		/// <param name="updateModel"></param>
		/// <param name="taxEvaluationContext"></param>
		/// <returns></returns>
		ICartBuilder AddOrUpdateShipment(Domain.Cart.Model.Shipment shipment);

		/// <summary>
		/// Remove exist shipment from cart
		/// </summary>
		/// <param name="shipmentId"></param>
		/// <returns></returns>
		ICartBuilder RemoveShipment(string shipmentId);

		/// <summary>
		/// Add or update payment in cart
		/// </summary>
		/// <param name="updateModel"></param>
		/// <returns></returns>
		ICartBuilder AddOrUpdatePayment(Payment updateModel);

		/// <summary>
		/// Merge other cart with captured
		/// </summary>
		/// <param name="cart"></param>
		/// <returns></returns>
		ICartBuilder MergeWithCart(ShoppingCart cart);

		/// <summary>
		/// Remove cart from service
		/// </summary>
		/// <returns></returns>
		ICartBuilder RemoveCart();

		///// <summary>
		///// Fill current captured cart from RFQ
		///// </summary>
		///// <param name="quoteRequest"></param>
		///// <returns></returns>
		//ICartBuilder> FillFromQuoteRequest(QuoteRequest quoteRequest);

		/// <summary>
		/// Returns all available shipment methods for current cart
		/// </summary>
		/// <returns></returns>
		ICollection<ShippingRate> GetAvailableShippingRates();

		/// <summary>
		/// Returns all available payment methods for current cart
		/// </summary>
		/// <returns></returns>
		ICollection<Domain.Payment.Model.PaymentMethod> GetAvailablePaymentMethods();

		/// <summary>
		/// Evaluate marketing discounts for captured cart
		/// </summary>
		/// <returns></returns>
		ICartBuilder EvaluatePromotions();

		/// <summary>
		/// Evaluate taxes  for captured cart
		/// </summary>
		/// <returns></returns>
		ICartBuilder EvaluateTax();

		/// <summary>
		/// Create order
		/// </summary>
		/// <param name="bankCardInfo"></param>
		/// <returns></returns>
		CreateOrderResult CreateOrder(BankCardInfo bankCardInfo);

		//Save cart changes
		void Save();

		ShoppingCart Cart { get; }

		Store Store { get; }
	}
}
