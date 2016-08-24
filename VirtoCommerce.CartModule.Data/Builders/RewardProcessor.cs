using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Marketing.Model;

namespace VirtoCommerce.CartModule.Data.Builders
{
	public static class RewardProcessor
	{
		public static void ApplyRewards(this ShoppingCart shoppingCart, IEnumerable<PromotionReward> rewards)
		{
			shoppingCart.Discounts?.Clear();

			var cartRewards = rewards.OfType<CartSubtotalReward>().Where(x => x.IsValid).ToList();
			foreach (var reward in cartRewards)
			{
				var discount = reward.ToDiscountModel(shoppingCart.Currency, shoppingCart.SubTotal, shoppingCart.SubTotal + shoppingCart.TaxTotal);
				if (shoppingCart.Discounts == null)
				{
					shoppingCart.Discounts = new List<Discount>();
				}

				shoppingCart.Discounts.Add(discount);
			}

			var lineItemRewards = rewards.OfType<CatalogItemAmountReward>().Where(x => x.IsValid).ToList();
			foreach (var lineItem in shoppingCart.Items)
			{
				lineItem.ApplyRewards(lineItemRewards);
			}

			var shipmentRewards = rewards.OfType<ShipmentReward>().Where(x => x.IsValid).ToList();
			foreach (var shipment in shoppingCart.Shipments)
			{
				shipment.ApplyRewards(shipmentRewards);
			}

			if (shoppingCart.Coupon != null && !string.IsNullOrEmpty(shoppingCart.Coupon.Code))
			{
				var couponRewards = rewards.Where(r => r.Promotion.Coupons != null && r.Promotion.Coupons.Any()).ToList();

				if (!couponRewards.Any())
				{
					//shoppingCart.Coupon.IsValid = false;
					shoppingCart.Coupon.InvalidDescription = "InvalidCouponCode";
				}

				foreach (var reward in couponRewards)
				{
					var couponCode = reward.Promotion.Coupons.FirstOrDefault(c => c == shoppingCart.Coupon.Code);
					if (!string.IsNullOrEmpty(couponCode))
					{
						//shoppingCart.Coupon.IsValid = reward.IsValid;
						//shoppingCart.Coupon.Description = reward.Promotion.Description;
						shoppingCart.Coupon.InvalidDescription = null;
					}
				}
			}
		}

		public static void ApplyRewards(this Shipment shipment, IEnumerable<ShipmentReward> shipmentRewards)
		{
			var rewards = shipmentRewards.Where(r => string.IsNullOrEmpty(r.ShippingMethod) || string.Equals(r.ShippingMethod, shipment.ShipmentMethodCode, StringComparison.InvariantCultureIgnoreCase));

			shipment.Discounts?.Clear();

			foreach (var reward in rewards)
			{
				var discount = reward.ToDiscountModel(shipment.Currency, shipment.ShippingPrice, shipment.ShippingPrice + shipment.TaxTotal);
				if (reward.IsValid)
				{
					if (shipment.Discounts == null)
					{
						shipment.Discounts = new List<Discount>();
					}

					shipment.Discounts.Add(discount);
				}
			}
		}

		public static void ApplyRewards(this LineItem lineItem, IEnumerable<CatalogItemAmountReward> catalogItemAmountRewards)
		{
			var lineItemRewards = catalogItemAmountRewards.Where(r => string.IsNullOrEmpty(r.ProductId) || string.Equals(r.ProductId, lineItem.ProductId, StringComparison.OrdinalIgnoreCase));

			lineItem.Discounts?.Clear();

			foreach (var reward in lineItemRewards)
			{
				var discount = reward.ToDiscountModel(lineItem.Currency, lineItem.SalePrice, lineItem.SalePrice + lineItem.TaxTotal);

				if (reward.Quantity > 0)
				{
					var discountAmount = discount.DiscountAmount * Math.Min(reward.Quantity, lineItem.Quantity);
					var discountAmountWithTax = discount.DiscountAmountWithTax * Math.Min(reward.Quantity, lineItem.Quantity);
					
					//TODO: need allocate more rightly between each quantities
					discount.DiscountAmount = discountAmount / lineItem.Quantity;
					discount.DiscountAmountWithTax = discountAmountWithTax / lineItem.Quantity;
				}

				if (reward.IsValid)
				{
					if (lineItem.Discounts == null)
					{
						lineItem.Discounts = new List<Discount>();
					}

					lineItem.Discounts.Add(discount);
				}
			}
		}

		public static void ApplyRewards(this Model.ShippingRate shippingRate, IEnumerable<PromotionReward> rewards)
		{
			var shipmentRewards = rewards.OfType<ShipmentReward>().Where(r => string.IsNullOrEmpty(r.ShippingMethod) || string.Equals(r.ShippingMethod, shippingRate.ShippingMethod.Code, StringComparison.InvariantCulture));

			shippingRate.Discounts?.Clear();

			foreach (var reward in shipmentRewards)
			{
				var discount = reward.ToDiscountModel(shippingRate.Currency, shippingRate.Rate, shippingRate.Rate + shippingRate.TaxTotal);

				if (reward.IsValid)
				{
					if (shippingRate.Discounts == null)
					{
						shippingRate.Discounts = new List<Discount>();
					}

					shippingRate.Discounts.Add(discount);
				}
			}
		}

		public static Discount ToDiscountModel(this AmountBasedReward amountBasedReward, string currency, decimal amount, decimal amountWithTaxes)
		{
			var discount = new Discount
			{
				Currency = currency,
				DiscountAmount = GetDiscountAmount(amountBasedReward.AmountType, amountBasedReward.Amount, amount),
				DiscountAmountWithTax = GetDiscountAmount(amountBasedReward.AmountType, amountBasedReward.Amount, amountWithTaxes),
				Description = amountBasedReward.Promotion.Description,
				PromotionId = amountBasedReward.Promotion.Id
			};

			return discount;
		}

		public static Discount ToDiscountModel(this CartSubtotalReward cartSubtotalReward, string currency, decimal amount, decimal amountWithTaxes)
		{
			var discount = new Discount
			{
				Currency = currency,
				DiscountAmount = GetDiscountAmount(RewardAmountType.Absolute, cartSubtotalReward.Amount, amount),
				DiscountAmountWithTax = GetDiscountAmount(RewardAmountType.Absolute, cartSubtotalReward.Amount, amountWithTaxes),
				Description = cartSubtotalReward.Promotion.Description,
				PromotionId = cartSubtotalReward.Promotion.Id
			};

			return discount;
		}

		public static decimal GetDiscountAmount(RewardAmountType rewardAmountType, decimal amount, decimal originalAmount)
		{
			var absoluteAmount = amount;

			if (rewardAmountType == RewardAmountType.Relative)
			{
				absoluteAmount = amount * originalAmount / 100;
			}

			return absoluteAmount;
		}
	}
}
