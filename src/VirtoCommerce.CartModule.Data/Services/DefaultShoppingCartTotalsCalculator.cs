using System;
using System.Collections.Generic;
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
            ArgumentNullException.ThrowIfNull(cart);

            var cartItemsWithoutGifts = cart.Items?.Where(x => !x.IsGift).ToList();

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
            cart.FeeTotalWithTax = 0m;
            cart.TaxTotal = 0m;

            var cartsByCurrency = new Dictionary<string, ShoppingCart>
            {
                { cart.Currency, cart }
            };

            var currencyCodes = (cart.Items?.Select(x => x.Currency) ?? [])
                .Concat(cart.Shipments?.Select(x => x.Currency) ?? [])
                .Concat(cart.Payments?.Select(x => x.Currency) ?? [])
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct();

            foreach (var currencyCode in currencyCodes)
            {
                AddShoppingCartByCurrency(cartsByCurrency, currencyCode);
            }

            var selectedItemsWithoutGifts = cartItemsWithoutGifts?.Where(x => x.SelectedForCheckout).ToList();
            foreach (var (currencyCode, currencyCart) in cartsByCurrency)
            {
                var currencyItems = selectedItemsWithoutGifts?.Where(x => x.Currency == currencyCode).ToList() ?? [];
                currencyCart.SubTotal = currencyItems.Sum(x => x.ListTotal);
                currencyCart.SubTotalWithTax = currencyItems.Sum(x => x.ListTotalWithTax);
                currencyCart.SubTotalDiscount = currencyItems.Sum(x => x.DiscountTotal);
                currencyCart.SubTotalDiscountWithTax = currencyItems.Sum(x => x.DiscountTotalWithTax);
                currencyCart.DiscountTotal += currencyItems.Sum(x => x.DiscountTotal);
                currencyCart.DiscountTotalWithTax += currencyItems.Sum(x => x.DiscountTotalWithTax);
                currencyCart.FeeTotal += currencyItems.Sum(x => x.Fee);
                currencyCart.FeeTotalWithTax += currencyItems.Sum(x => x.FeeWithTax);
                currencyCart.TaxTotal += currencyItems.Sum(x => x.TaxTotal);

                var currencyShipments = cart.Shipments?.Where(x => x.Currency == currencyCode).ToList() ?? [];
                currencyCart.ShippingTotal = currencyShipments.Sum(x => x.Total);
                currencyCart.ShippingTotalWithTax = currencyShipments.Sum(x => x.TotalWithTax);
                currencyCart.ShippingSubTotal = currencyShipments.Sum(x => x.Price);
                currencyCart.ShippingSubTotalWithTax = currencyShipments.Sum(x => x.PriceWithTax);
                currencyCart.ShippingDiscountTotal = currencyShipments.Sum(x => x.DiscountAmount);
                currencyCart.ShippingDiscountTotalWithTax = currencyShipments.Sum(x => x.DiscountAmountWithTax);
                currencyCart.DiscountTotal += currencyShipments.Sum(x => x.DiscountAmount);
                currencyCart.DiscountTotalWithTax += currencyShipments.Sum(x => x.DiscountAmountWithTax);
                currencyCart.FeeTotal += currencyShipments.Sum(x => x.Fee);
                currencyCart.FeeTotalWithTax += currencyShipments.Sum(x => x.FeeWithTax);
                currencyCart.TaxTotal += currencyShipments.Sum(x => x.TaxTotal);

                var currencyPayments = cart.Payments?.Where(x => x.Currency == currencyCode).ToList() ?? [];
                currencyCart.PaymentTotal = currencyPayments.Sum(x => x.Total);
                currencyCart.PaymentTotalWithTax = currencyPayments.Sum(x => x.TotalWithTax);
                currencyCart.PaymentSubTotal = currencyPayments.Sum(x => x.Price);
                currencyCart.PaymentSubTotalWithTax = currencyPayments.Sum(x => x.PriceWithTax);
                currencyCart.PaymentDiscountTotal = currencyPayments.Sum(x => x.DiscountAmount);
                currencyCart.PaymentDiscountTotalWithTax = currencyPayments.Sum(x => x.DiscountAmountWithTax);
                currencyCart.DiscountTotal += currencyPayments.Sum(x => x.DiscountAmount);
                currencyCart.DiscountTotalWithTax += currencyPayments.Sum(x => x.DiscountAmountWithTax);
                currencyCart.TaxTotal += currencyPayments.Sum(x => x.TaxTotal);
            }

            var allCurrencies = _currencyService.GetAllCurrenciesAsync().GetAwaiter().GetResult().ToList();

            foreach (var currencyCart in cartsByCurrency.Select(x => x.Value))
            {
                var taxFactor = 1 + currencyCart.TaxPercentRate;
                currencyCart.FeeWithTax = currencyCart.Fee * taxFactor;
                currencyCart.FeeTotalWithTax = currencyCart.FeeTotal * taxFactor;
                currencyCart.DiscountTotal += currencyCart.DiscountAmount;
                currencyCart.DiscountTotalWithTax += currencyCart.DiscountAmount * taxFactor;
                //Subtract from cart tax total self discount tax amount
                currencyCart.TaxTotal -= currencyCart.DiscountAmount * currencyCart.TaxPercentRate;

                //Need to round all cart totals
                var currency = allCurrencies.First(c => c.Code == currencyCart.Currency);
                currencyCart.SubTotal = currency.RoundingPolicy.RoundMoney(currencyCart.SubTotal, currency);
                currencyCart.SubTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.SubTotalWithTax, currency);
                currencyCart.SubTotalDiscount = currency.RoundingPolicy.RoundMoney(currencyCart.SubTotalDiscount, currency);
                currencyCart.SubTotalDiscountWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.SubTotalDiscountWithTax, currency);
                currencyCart.TaxTotal = currency.RoundingPolicy.RoundMoney(currencyCart.TaxTotal, currency);
                currencyCart.DiscountTotal = currency.RoundingPolicy.RoundMoney(currencyCart.DiscountTotal, currency);
                currencyCart.DiscountTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.DiscountTotalWithTax, currency);
                currencyCart.Fee = currency.RoundingPolicy.RoundMoney(currencyCart.Fee, currency);
                currencyCart.FeeWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.FeeWithTax, currency);
                currencyCart.FeeTotal = currency.RoundingPolicy.RoundMoney(currencyCart.FeeTotal, currency);
                currencyCart.FeeTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.FeeTotalWithTax, currency);
                currencyCart.ShippingTotal = currency.RoundingPolicy.RoundMoney(currencyCart.ShippingTotal, currency);
                currencyCart.ShippingTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.ShippingTotalWithTax, currency);
                currencyCart.ShippingSubTotal = currency.RoundingPolicy.RoundMoney(currencyCart.ShippingSubTotal, currency);
                currencyCart.ShippingSubTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.ShippingSubTotalWithTax, currency);
                currencyCart.PaymentTotal = currency.RoundingPolicy.RoundMoney(currencyCart.PaymentTotal, currency);
                currencyCart.PaymentTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.PaymentTotalWithTax, currency);
                currencyCart.PaymentSubTotal = currency.RoundingPolicy.RoundMoney(currencyCart.PaymentSubTotal, currency);
                currencyCart.PaymentSubTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.PaymentSubTotalWithTax, currency);
                currencyCart.PaymentDiscountTotal = currency.RoundingPolicy.RoundMoney(currencyCart.PaymentDiscountTotal, currency);
                currencyCart.PaymentDiscountTotalWithTax = currency.RoundingPolicy.RoundMoney(currencyCart.PaymentDiscountTotalWithTax, currency);

                currencyCart.Total = currencyCart.SubTotal + currencyCart.ShippingSubTotal + currencyCart.TaxTotal + currencyCart.PaymentSubTotal + currencyCart.FeeTotal - currencyCart.DiscountTotal;
            }

            cart.LineItemsCount = cartItemsWithoutGifts?.Count ?? 0;

            cart.CartTotals = cartsByCurrency.Select(x =>
            {
                var cartTotal = AbstractTypeFactory<CartTotal>.TryCreateInstance();

                cartTotal.CurrencyCode = x.Value.Currency;
                cartTotal.Total = x.Value.Total;
                cartTotal.SubTotal = x.Value.SubTotal;
                cartTotal.TaxTotal = x.Value.TaxTotal;
                cartTotal.DiscountTotal = x.Value.DiscountTotal;

                return cartTotal;
            }).ToList();
        }

        protected virtual void CalculatePaymentTotals(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment);

            var taxFactor = 1 + payment.TaxPercentRate;
            payment.Total = payment.Price - payment.DiscountAmount;
            payment.TotalWithTax = payment.Total * taxFactor;
            payment.PriceWithTax = payment.Price * taxFactor;
            payment.DiscountAmountWithTax = payment.DiscountAmount * taxFactor;
            payment.TaxTotal = payment.Total * payment.TaxPercentRate;
        }

        protected virtual void CalculateShipmentTotals(Shipment shipment)
        {
            ArgumentNullException.ThrowIfNull(shipment);

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
            ArgumentNullException.ThrowIfNull(lineItem);

            var quantity = Math.Max(1, lineItem.Quantity);
            var currency = _currencyService.GetAllCurrenciesAsync().GetAwaiter().GetResult().First(c => c.Code == lineItem.Currency);

            lineItem.ListTotal = lineItem.ListPrice * quantity;
            lineItem.PlacedPrice = lineItem.ListPrice - lineItem.DiscountAmount;
            lineItem.DiscountTotal = currency.RoundingPolicy.RoundMoney(lineItem.DiscountAmount * quantity, currency);
            lineItem.ExtendedPrice = lineItem.ListTotal - lineItem.DiscountTotal;

            var taxFactor = 1 + lineItem.TaxPercentRate;

            lineItem.ListPriceWithTax = lineItem.ListPrice * taxFactor;
            lineItem.ListTotalWithTax = lineItem.ListTotal * taxFactor;
            lineItem.SalePriceWithTax = lineItem.SalePrice * taxFactor;
            lineItem.PlacedPriceWithTax = lineItem.PlacedPrice * taxFactor;
            lineItem.ExtendedPriceWithTax = lineItem.ExtendedPrice * taxFactor;
            lineItem.DiscountAmountWithTax = lineItem.DiscountAmount * taxFactor;
            lineItem.DiscountTotalWithTax = lineItem.DiscountTotal * taxFactor;
            lineItem.FeeWithTax = lineItem.Fee * taxFactor;

            lineItem.TaxTotal = (lineItem.ExtendedPrice + lineItem.Fee) * lineItem.TaxPercentRate;
        }

        private static ShoppingCart AddShoppingCartByCurrency(Dictionary<string, ShoppingCart> cartByCurrency, string currencyCode)
        {
            if (!cartByCurrency.TryGetValue(currencyCode, out var currencyCart))
            {
                currencyCart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();
                currencyCart.Currency = currencyCode;
                cartByCurrency.Add(currencyCode, currencyCart);
            }

            return currencyCart;
        }
    }
}
