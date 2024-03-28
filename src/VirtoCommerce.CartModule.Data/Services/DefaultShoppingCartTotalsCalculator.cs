using System;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Services
{
    /// <summary>
    /// Respond for totals values calculation for Shopping cart and all nested objects
    /// </summary>
    public class DefaultShoppingCartTotalsCalculator : IShoppingCartTotalsCalculator
    {
        private readonly ICurrencyService _currencyService;

        public DefaultShoppingCartTotalsCalculator(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// Cart subtotal discount
        /// When a discount is applied to the cart subtotal, the tax calculation has already been applied, and is reflected in the tax subtotal.
        /// Therefore, a discount applying to the cart subtotal will occur after tax.
        /// For instance, if the cart subtotal is $100, and $15 is the tax subtotal, a cart - wide discount of 10 % will yield a total of $105($100 subtotal – $10 discount + $15 tax on the original $100).
        /// </summary>
        public virtual void CalculateTotals(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            var cartItemsWithoutGifts = cart.Items?.Where(x => !x.IsGift);

            //Calculate totals for line items
            foreach (var item in cartItemsWithoutGifts ?? Enumerable.Empty<LineItem>())
            {
                CalculateLineItemTotals(item);
            }

            //Calculate totals for shipments
            if (!cart.Shipments.IsNullOrEmpty())
            {
                foreach (var shipment in cart.Shipments)
                {
                    CalculateShipmentTotals(shipment);
                }
            }

            //Calculate totals for payments
            if (!cart.Payments.IsNullOrEmpty())
            {
                foreach (var payment in cart.Payments)
                {
                    CalculatePaymentTotals(payment);
                }
            }

            cart.DiscountTotal = 0m;
            cart.DiscountTotalWithTax = 0m;
            cart.FeeTotal = cart.Fee;
            cart.TaxTotal = 0m;

            var selectedItemsWithoutGifts = cartItemsWithoutGifts?.Where(x => x.SelectedForCheckout);
            if (selectedItemsWithoutGifts != null)
            {
                cart.SubTotal = selectedItemsWithoutGifts.Sum(x => x.ListPrice * x.Quantity);
                cart.SubTotalWithTax = selectedItemsWithoutGifts.Sum(x => x.ListPriceWithTax * x.Quantity);
                cart.SubTotalDiscount = selectedItemsWithoutGifts.Sum(x => x.DiscountTotal);
                cart.SubTotalDiscountWithTax = selectedItemsWithoutGifts.Sum(x => x.DiscountTotalWithTax);
                cart.DiscountTotal += selectedItemsWithoutGifts.Sum(x => x.DiscountTotal);
                cart.DiscountTotalWithTax += selectedItemsWithoutGifts.Sum(x => x.DiscountTotalWithTax);
                cart.FeeTotal += selectedItemsWithoutGifts.Sum(x => x.Fee);
                cart.FeeTotalWithTax += selectedItemsWithoutGifts.Sum(x => x.FeeWithTax);
                cart.TaxTotal += selectedItemsWithoutGifts.Sum(x => x.TaxTotal);
            }

            if (cart.Shipments != null)
            {
                cart.ShippingTotal = cart.Shipments.Sum(x => x.Total);
                cart.ShippingTotalWithTax = cart.Shipments.Sum(x => x.TotalWithTax);
                cart.ShippingSubTotal = cart.Shipments.Sum(x => x.Price);
                cart.ShippingSubTotalWithTax = cart.Shipments.Sum(x => x.PriceWithTax);
                cart.ShippingDiscountTotal = cart.Shipments.Sum(x => x.DiscountAmount);
                cart.ShippingDiscountTotalWithTax = cart.Shipments.Sum(x => x.DiscountAmountWithTax);
                cart.DiscountTotal += cart.Shipments.Sum(x => x.DiscountAmount);
                cart.DiscountTotalWithTax += cart.Shipments.Sum(x => x.DiscountAmountWithTax);
                cart.FeeTotal += cart.Shipments.Sum(x => x.Fee);
                cart.FeeTotalWithTax += cart.Shipments.Sum(x => x.FeeWithTax);
                cart.TaxTotal += cart.Shipments.Sum(x => x.TaxTotal);
            }

            if (cart.Payments != null)
            {
                cart.PaymentTotal = cart.Payments.Sum(x => x.Total);
                cart.PaymentTotalWithTax = cart.Payments.Sum(x => x.TotalWithTax);
                cart.PaymentSubTotal = cart.Payments.Sum(x => x.Price);
                cart.PaymentSubTotalWithTax = cart.Payments.Sum(x => x.PriceWithTax);
                cart.PaymentDiscountTotal = cart.Payments.Sum(x => x.DiscountAmount);
                cart.PaymentDiscountTotalWithTax = cart.Payments.Sum(x => x.DiscountAmountWithTax);
                cart.DiscountTotal += cart.Payments.Sum(x => x.DiscountAmount);
                cart.DiscountTotalWithTax += cart.Payments.Sum(x => x.DiscountAmountWithTax);
                cart.TaxTotal += cart.Payments.Sum(x => x.TaxTotal);
            }

            var taxFactor = 1 + cart.TaxPercentRate;
            cart.FeeWithTax = cart.Fee * taxFactor;
            cart.FeeTotalWithTax = cart.FeeTotal * taxFactor;
            cart.DiscountTotal += cart.DiscountAmount;
            cart.DiscountTotalWithTax += cart.DiscountAmount * taxFactor;
            //Subtract from cart tax total self discount tax amount
            cart.TaxTotal -= cart.DiscountAmount * cart.TaxPercentRate;

            //Need to round all cart totals
            var currency = _currencyService.GetAllCurrenciesAsync().GetAwaiter().GetResult().First(c => c.Code == cart.Currency);
            cart.SubTotal = currency.RoundingPolicy.RoundMoney(cart.SubTotal, currency);
            cart.SubTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.SubTotalWithTax, currency);
            cart.SubTotalDiscount = currency.RoundingPolicy.RoundMoney(cart.SubTotalDiscount, currency);
            cart.SubTotalDiscountWithTax = currency.RoundingPolicy.RoundMoney(cart.SubTotalDiscountWithTax, currency);
            cart.TaxTotal = currency.RoundingPolicy.RoundMoney(cart.TaxTotal, currency);
            cart.DiscountTotal = currency.RoundingPolicy.RoundMoney(cart.DiscountTotal, currency);
            cart.DiscountTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.DiscountTotalWithTax, currency);
            cart.Fee = currency.RoundingPolicy.RoundMoney(cart.Fee, currency);
            cart.FeeWithTax = currency.RoundingPolicy.RoundMoney(cart.FeeWithTax, currency);
            cart.FeeTotal = currency.RoundingPolicy.RoundMoney(cart.FeeTotal, currency);
            cart.FeeTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.FeeTotalWithTax, currency);
            cart.ShippingTotal = currency.RoundingPolicy.RoundMoney(cart.ShippingTotal, currency);
            cart.ShippingTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.ShippingTotalWithTax, currency);
            cart.ShippingSubTotal = currency.RoundingPolicy.RoundMoney(cart.ShippingSubTotal, currency);
            cart.ShippingSubTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.ShippingSubTotalWithTax, currency);
            cart.PaymentTotal = currency.RoundingPolicy.RoundMoney(cart.PaymentTotal, currency);
            cart.PaymentTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.PaymentTotalWithTax, currency);
            cart.PaymentSubTotal = currency.RoundingPolicy.RoundMoney(cart.PaymentSubTotal, currency);
            cart.PaymentSubTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.PaymentSubTotalWithTax, currency);
            cart.PaymentDiscountTotal = currency.RoundingPolicy.RoundMoney(cart.PaymentDiscountTotal, currency);
            cart.PaymentDiscountTotalWithTax = currency.RoundingPolicy.RoundMoney(cart.PaymentDiscountTotalWithTax, currency);

            cart.Total = cart.SubTotal + cart.ShippingSubTotal + cart.TaxTotal + cart.PaymentSubTotal + cart.FeeTotal - cart.DiscountTotal;

            cart.LineItemsCount = cart.Items?.Count ?? 0;
        }

        protected virtual void CalculatePaymentTotals(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }
            var taxFactor = 1 + payment.TaxPercentRate;
            payment.Total = payment.Price - payment.DiscountAmount;
            payment.TotalWithTax = payment.Total * taxFactor;
            payment.PriceWithTax = payment.Price * taxFactor;
            payment.DiscountAmountWithTax = payment.DiscountAmount * taxFactor;
            payment.TaxTotal = payment.Total * payment.TaxPercentRate;
        }

        protected virtual void CalculateShipmentTotals(Shipment shipment)
        {
            if (shipment == null)
            {
                throw new ArgumentNullException(nameof(shipment));
            }
            var taxFactor = 1 + shipment.TaxPercentRate;
            shipment.PriceWithTax = shipment.Price * taxFactor;
            shipment.DiscountAmountWithTax = shipment.DiscountAmount * taxFactor;
            shipment.FeeWithTax = shipment.Fee * taxFactor;
            shipment.Total = shipment.Price + shipment.Fee - shipment.DiscountAmount;
            shipment.TotalWithTax = shipment.PriceWithTax + shipment.FeeWithTax - shipment.DiscountAmountWithTax;
            shipment.TaxTotal = shipment.Total * shipment.TaxPercentRate;
        }

        protected virtual void CalculateLineItemTotals(LineItem lineItem)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }
            var taxFactor = 1 + lineItem.TaxPercentRate;
            lineItem.ListPriceWithTax = lineItem.ListPrice * taxFactor;
            lineItem.SalePriceWithTax = lineItem.SalePrice * taxFactor;
            lineItem.PlacedPrice = lineItem.ListPrice - lineItem.DiscountAmount;
            lineItem.PlacedPriceWithTax = lineItem.PlacedPrice * taxFactor;
            lineItem.ExtendedPrice = lineItem.PlacedPrice * lineItem.Quantity;
            lineItem.DiscountAmountWithTax = lineItem.DiscountAmount * taxFactor;
            lineItem.DiscountTotal = lineItem.DiscountAmount * Math.Max(1, lineItem.Quantity);
            lineItem.FeeWithTax = lineItem.Fee * taxFactor;
            lineItem.PlacedPriceWithTax = lineItem.PlacedPrice * taxFactor;
            lineItem.ExtendedPriceWithTax = lineItem.PlacedPriceWithTax * lineItem.Quantity;
            lineItem.DiscountTotalWithTax = lineItem.DiscountAmountWithTax * Math.Max(1, lineItem.Quantity);
            lineItem.TaxTotal = (lineItem.ExtendedPrice + lineItem.Fee) * lineItem.TaxPercentRate;
        }
    }
}
