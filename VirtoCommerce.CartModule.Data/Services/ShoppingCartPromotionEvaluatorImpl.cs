using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.Domain.Shipping.Model;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartPromotionEvaluatorImpl : IShoppingCartPromotionEvaluator
    {
        private readonly IMarketingPromoEvaluator _marketingPromoEvaluator;
        public ShoppingCartPromotionEvaluatorImpl(IMarketingPromoEvaluator marketingPromoEvaluator)
        {
            _marketingPromoEvaluator = marketingPromoEvaluator;
        }

        #region IShoppingCartPromotionEvaluator Members
        public virtual void EvaluatePromotions(ShoppingCart cart)
        {
            var promotionEvaluationContext = CartToPromotionEvaluationContext(cart);
            var promotionResult = _marketingPromoEvaluator.EvaluatePromotion(promotionEvaluationContext);
            if (promotionResult.Rewards != null)
            {
                ApplyRewards(cart, promotionResult.Rewards);
            }
        }

        public virtual void EvaluatePromotions(ShoppingCart cart, IEnumerable<ShippingRate> shippingRates)
        {
            var promotionEvaluationContext = CartToPromotionEvaluationContext(cart);
            var promotionResult = _marketingPromoEvaluator.EvaluatePromotion(promotionEvaluationContext);
            if (promotionResult.Rewards != null)
            {
                foreach (var rate in shippingRates)
                {
                    ApplyRewards(rate, promotionResult.Rewards);
                }
            }
        } 
        #endregion

        protected virtual PromotionEvaluationContext CartToPromotionEvaluationContext(ShoppingCart cart)
        {
            List<ProductPromoEntry> promotionItems = new List<ProductPromoEntry>();

            if (cart.Items != null)
            {
                foreach (var lineItem in cart.Items)
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
                CartTotal = cart.Total,
                Coupon = cart.Coupon?.Code,
                Currency = cart.Currency,
                CustomerId = cart.CustomerId,
                //todo: IsRegisteredUser = cart.Customer.IsRegisteredUser,
                Language = cart.LanguageCode,
                PromoEntries = promotionItems,
                StoreId = cart.StoreId
            };

            return retVal;
        }

        protected virtual void ApplyRewards(ShoppingCart shoppingCart, IEnumerable<PromotionReward> rewards)
        {
            if (shoppingCart.Discounts != null)
            {
                shoppingCart.Discounts.Clear();
            }

            var cartRewards = rewards.OfType<CartSubtotalReward>().Where(x => x.IsValid).ToList();
            foreach (var reward in cartRewards)
            {
                var discount = ToDiscountModel(reward, shoppingCart.Currency, shoppingCart.SubTotal, shoppingCart.SubTotal + shoppingCart.TaxTotal);
                if (shoppingCart.Discounts == null)
                {
                    shoppingCart.Discounts = new List<Discount>();
                }
                shoppingCart.Discounts.Add(discount);
                shoppingCart.DiscountAmount = discount.DiscountAmount;
                shoppingCart.DiscountAmountWithTax = discount.DiscountAmountWithTax;
            }

            if (shoppingCart.Items != null)
            {
                var lineItemRewards = rewards.OfType<CatalogItemAmountReward>().Where(x => x.IsValid).ToList();
                foreach (var lineItem in shoppingCart.Items)
                {
                    ApplyRewards(lineItem, lineItemRewards);
                }
            }

            if (shoppingCart.Shipments != null)
            {
                var shipmentRewards = rewards.OfType<ShipmentReward>().Where(x => x.IsValid).ToList();
                foreach (var shipment in shoppingCart.Shipments)
                {
                    ApplyRewards(shipment, shipmentRewards);
                }
            }

            if (shoppingCart.Coupon != null && !string.IsNullOrEmpty(shoppingCart.Coupon.Code))
            {
                var couponRewards = rewards.Where(r => r.Promotion.Coupons != null && r.Promotion.Coupons.Any()).ToList();

                if (!couponRewards.Any())
                {
                    shoppingCart.Coupon.IsValid = false;
                }

                foreach (var reward in couponRewards)
                {
                    var couponCode = reward.Promotion.Coupons.FirstOrDefault(c => c == shoppingCart.Coupon.Code);
                    if (!string.IsNullOrEmpty(couponCode))
                    {
                        shoppingCart.Coupon.IsValid = reward.IsValid;
                        shoppingCart.Coupon.InvalidDescription = null;
                    }
                }
            }
        }

        protected virtual void ApplyRewards(Shipment shipment, IEnumerable<ShipmentReward> shipmentRewards)
        {
            var rewards = shipmentRewards.Where(r => string.IsNullOrEmpty(r.ShippingMethod) || string.Equals(r.ShippingMethod, shipment.ShipmentMethodCode, StringComparison.InvariantCultureIgnoreCase));

            if (shipment.Discounts != null)
            {
                shipment.Discounts.Clear();
            }

            foreach (var reward in rewards)
            {
                var discount = ToDiscountModel(reward, shipment.Currency, shipment.ShippingPrice, shipment.ShippingPrice + shipment.TaxTotal);
                if (reward.IsValid)
                {
                    if (shipment.Discounts == null)
                    {
                        shipment.Discounts = new List<Discount>();
                    }

                    shipment.Discounts.Add(discount);
                    shipment.DiscountTotal = discount.DiscountAmount;
                    shipment.DiscountTotalWithTax = discount.DiscountAmountWithTax;
                }
            }
        }

        protected virtual void ApplyRewards(LineItem lineItem, IEnumerable<CatalogItemAmountReward> catalogItemAmountRewards)
        {
            var lineItemRewards = catalogItemAmountRewards.Where(r => string.IsNullOrEmpty(r.ProductId) || string.Equals(r.ProductId, lineItem.ProductId, StringComparison.OrdinalIgnoreCase));

            if (lineItem.Discounts != null)
            {
                lineItem.Discounts.Clear();
            }

            lineItem.DiscountAmount = Math.Max(0, lineItem.ListPrice - lineItem.SalePrice);
            lineItem.DiscountAmountWithTax = Math.Max(0, lineItem.ListPriceWithTax - lineItem.SalePriceWithTax);

            foreach (var reward in lineItemRewards)
            {
                var discount = ToDiscountModel(reward, lineItem.Currency, lineItem.SalePrice, lineItem.SalePrice + lineItem.TaxTotal);

                if (reward.Quantity > 0)
                {
                    var discountAmount = discount.DiscountAmount * Math.Min(reward.Quantity, lineItem.Quantity);
                    var discountAmountWithTax = discount.DiscountAmountWithTax * Math.Min(reward.Quantity, lineItem.Quantity);

                    //TODO: need allocate more rightly between each quantities
                    discount.DiscountAmount = discountAmount / lineItem.Quantity;
                    discount.DiscountAmountWithTax = discountAmountWithTax / lineItem.Quantity;
                    lineItem.DiscountAmount += discountAmount;
                    lineItem.DiscountAmountWithTax += discountAmountWithTax;
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

        protected virtual void ApplyRewards(ShippingRate shippingRate, IEnumerable<PromotionReward> rewards)
        {
            var shipmentRewards = rewards.OfType<ShipmentReward>().Where(r => string.IsNullOrEmpty(r.ShippingMethod) || string.Equals(r.ShippingMethod, shippingRate.ShippingMethod.Code, StringComparison.InvariantCulture));

            foreach (var reward in shipmentRewards)
            {
                var discount = ToDiscountModel(reward, shippingRate.Currency, shippingRate.Rate, shippingRate.RateWithTax);

                if (reward.IsValid)
                {
                    shippingRate.DiscountAmount += discount.DiscountAmount;
                    shippingRate.DiscountAmountWithTax += discount.DiscountAmountWithTax;
                }
            }
        }


        protected virtual Discount ToDiscountModel(AmountBasedReward amountBasedReward, string currency, decimal amount, decimal amountWithTaxes)
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

        protected virtual Discount ToDiscountModel(CartSubtotalReward cartSubtotalReward, string currency, decimal amount, decimal amountWithTaxes)
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

        protected virtual decimal GetDiscountAmount(RewardAmountType rewardAmountType, decimal amount, decimal originalAmount)
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
