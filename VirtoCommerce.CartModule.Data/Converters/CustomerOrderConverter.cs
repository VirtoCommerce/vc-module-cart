using System;
using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Payment.Model;

namespace VirtoCommerce.CartModule.Data.Converters
{
	public static class CustomerOrderConverter
	{
		public static CustomerOrder ToCustomerOrder(this ShoppingCart cart)
		{
			if (cart == null)
				throw new ArgumentNullException("cart");

			var retVal = new CustomerOrder
			{
				Id = Guid.NewGuid().ToString(),
				Currency = cart.Currency,
				CustomerId = cart.CustomerId,
				CustomerName = cart.CustomerName,
				StoreId = cart.StoreId,
				OrganizationId = cart.OrganizationId,
				Status = "New",
				Addresses = new List<Address>()
			};

			if (cart.Items != null)
			{
				retVal.Items = cart.Items.Select(x => x.ToOrderCoreModel()).ToList();
			}

			if (cart.Discounts != null)
			{
				retVal.Discount = cart.Discounts.Select(x => x.ToOrderCoreModel()).FirstOrDefault();
			}

			if (cart.Addresses != null)
			{
				retVal.Addresses = cart.Addresses.Select(x => x.ToCoreModel()).ToList();
			}

			if (cart.Shipments != null)
			{
				retVal.Shipments = cart.Shipments.Select(x => x.ToOrderCoreModel()).ToList();
				//Add shipping address to order
				retVal.Addresses.AddRange(retVal.Shipments.Where(x => x.DeliveryAddress != null).Select(x => x.DeliveryAddress));
				//Redistribute order line items to shipment if cart shipment items empty 
				var shipment = retVal.Shipments.FirstOrDefault();
				if (shipment != null && shipment.Items.IsNullOrEmpty())
				{
					shipment.Items = retVal.Items.Select(x => new Domain.Order.Model.ShipmentItem { LineItem = x, Quantity = x.Quantity }).ToList();
				}
			}
			if (cart.Payments != null)
			{
				retVal.InPayments = new List<PaymentIn>();
				foreach (var payment in cart.Payments)
				{
					var paymentIn = payment.ToOrderCoreModel();
					if (paymentIn.BillingAddress != null)
					{
						//Add billing address to order
						retVal.Addresses.Add(paymentIn.BillingAddress);
					}
					paymentIn.CustomerId = cart.CustomerId;
					retVal.InPayments.Add(paymentIn);
				}
			}

			//Save only disctinct addresses for order
			retVal.Addresses = retVal.Addresses.Distinct().ToList();
			retVal.TaxDetails = cart.TaxDetails;
			retVal.Tax = cart.TaxTotal;
			retVal.TaxIncluded = cart.TaxIncluded ?? false;
			retVal.Sum = cart.Total;
			return retVal;
		}

		public static Domain.Order.Model.LineItem ToOrderCoreModel(this Domain.Cart.Model.LineItem lineItem)
		{
			if (lineItem == null)
				throw new ArgumentNullException("lineItem");

			var retVal = new Domain.Order.Model.LineItem();
			retVal.InjectFrom(lineItem);
			retVal.Id = null;

			retVal.IsGift = lineItem.IsGift;
			retVal.BasePrice = lineItem.ListPrice;
			retVal.Price = lineItem.PlacedPrice;
			retVal.DiscountAmount = lineItem.DiscountTotal;
			retVal.Tax = lineItem.TaxTotal;
			retVal.FulfillmentLocationCode = lineItem.FulfillmentLocationCode;
			retVal.DynamicProperties = null; //to prevent copy dynamic properties from ShoppingCart LineItem to Order LineItem

			if (lineItem.Discounts != null)
			{
				retVal.Discount = lineItem.Discounts.Select(x => x.ToOrderCoreModel()).FirstOrDefault();
			}
			retVal.TaxDetails = lineItem.TaxDetails;
			return retVal;
		}

		public static Domain.Order.Model.Discount ToOrderCoreModel(this Domain.Cart.Model.Discount discount)
		{
			if (discount == null)
				throw new ArgumentNullException("discount");

			var retVal = new Domain.Order.Model.Discount();
			retVal.InjectFrom(discount);
			retVal.Currency = discount.Currency;

			return retVal;
		}

		public static Address ToCoreModel(this Address address)
		{
			if (address == null)
				throw new ArgumentNullException("entity");

			var retVal = new Address();
			retVal.InjectFrom(address);
			retVal.AddressType = (AddressType)(int)address.AddressType;
			return retVal;
		}

		public static Domain.Order.Model.Shipment ToOrderCoreModel(this Domain.Cart.Model.Shipment shipment)
		{
			var retVal = new Domain.Order.Model.Shipment();
			retVal.InjectFrom(shipment);
			retVal.Id = null;
			retVal.Currency = shipment.Currency;
			retVal.Sum = shipment.Total;
			retVal.Tax = shipment.TaxTotal;
			retVal.DiscountAmount = shipment.DiscountTotal;
			retVal.Status = "New";
			if (shipment.DeliveryAddress != null)
			{
				retVal.DeliveryAddress = shipment.DeliveryAddress.ToCoreModel();
			}
			if (shipment.Items != null)
			{
				retVal.Items = shipment.Items.Select(x => x.ToOrderCoreModel()).ToList();
			}
			if (shipment.Discounts != null)
			{
				retVal.Discount = shipment.Discounts.Select(x => x.ToOrderCoreModel()).FirstOrDefault();
			}
			retVal.TaxDetails = shipment.TaxDetails;
			return retVal;
		}

		public static Domain.Order.Model.ShipmentItem ToOrderCoreModel(this Domain.Cart.Model.ShipmentItem shipmentItem)
		{
			if (shipmentItem == null)
				throw new ArgumentNullException("shipmentItem");

			var retVal = new Domain.Order.Model.ShipmentItem();
			retVal.InjectFrom(shipmentItem);

			retVal.LineItem = shipmentItem.LineItem.ToOrderCoreModel();
			return retVal;
		}

		public static Domain.Order.Model.PaymentIn ToOrderCoreModel(this Domain.Cart.Model.Payment payment)
		{
			if (payment == null)
				throw new ArgumentNullException("payment");

			var retVal = new Domain.Order.Model.PaymentIn();
			retVal.InjectFrom(payment);
			retVal.Id = null;
			retVal.Currency = payment.Currency;
			retVal.GatewayCode = payment.PaymentGatewayCode;
			retVal.Sum = payment.Amount;
			retVal.PaymentStatus = PaymentStatus.New;

			if (payment.BillingAddress != null)
			{
				retVal.BillingAddress = payment.BillingAddress.ToCoreModel();
			}

			return retVal;
		}
	}
}