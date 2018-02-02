using System.Collections.Generic;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Domain.Cart.Model;
using Xunit;

namespace VirtoCommerce.CartModule.Test
{
    [Trait("Category", "CI")]
    public class OrderTotalsCalculationTest
    {
        [Fact]
        public void CalculateTotals_Should_Be_RightTotals()
        {
            var item1 = new LineItem { ListPrice = 10.99m, SalePrice = 9.66m, DiscountAmount = 1.33m, TaxPercentRate = 0.12m, Fee = 0.33m, Quantity = 2 };
            var item2 = new LineItem { ListPrice = 55.22m, SalePrice = 49.33m, DiscountAmount = 5.89m, TaxPercentRate = 0.12m, Fee = 0.12m, Quantity = 5 };
            var item3 = new LineItem { ListPrice = 88.45m, SalePrice = 77.67m, DiscountAmount = 10.78m, TaxPercentRate = 0.12m, Fee = 0.08m, Quantity = 12 };
            var payment = new Payment { Price = 44.52m, DiscountAmount = 10, TaxPercentRate = 0.12m };
            var shipment = new Shipment { Price = 22.0m, DiscountAmount = 5m, TaxPercentRate = 0.12m };

            var cart = new ShoppingCart
            {
                TaxPercentRate = 0.12m,
                Fee = 13.11m,
                Items = new List<LineItem> { item1, item2, item3 },
                Payments = new List<Payment> { payment },
                Shipments = new List<Shipment> { shipment }
            };
            var totalsCalculator = new DefaultShopingCartTotalsCalculator();
            totalsCalculator.CalculateTotals(cart);

            Assert.Equal(item1.ListPriceWithTax, 12.3088m);
            Assert.Equal(item1.SalePriceWithTax, 10.8192m);
            Assert.Equal(item1.PlacedPrice, 9.66m);
            Assert.Equal(item1.ExtendedPrice, 19.32m);
            Assert.Equal(item1.DiscountAmountWithTax, 1.4896m);
            Assert.Equal(item1.DiscountTotal, 2.66m);
            Assert.Equal(item1.FeeWithTax, 0.3696m);
            Assert.Equal(item1.PlacedPriceWithTax, 10.8192m);
            Assert.Equal(item1.ExtendedPriceWithTax, 21.6384m);
            Assert.Equal(item1.DiscountTotalWithTax, 2.9792m);
            Assert.Equal(item1.TaxTotal, 2.358m);

            Assert.Equal(shipment.DiscountAmountWithTax, 5.6m);
            Assert.Equal(shipment.PriceWithTax, 24.64m);
            Assert.Equal(shipment.FeeWithTax, 0.0m);
            Assert.Equal(shipment.Total, 17.0m);
            Assert.Equal(shipment.TotalWithTax, 19.04m);
            Assert.Equal(shipment.TaxTotal, 2.04m);

            Assert.Equal(payment.Total, 34.52m);
            Assert.Equal(payment.PriceWithTax, 49.8624m);
            Assert.Equal(payment.TotalWithTax, 38.6624m);
            Assert.Equal(payment.DiscountAmountWithTax, 11.2m);
            Assert.Equal(payment.TaxTotal, 4.1424m);

            Assert.Equal(cart.SubTotal, 1359.48m);
            Assert.Equal(cart.SubTotalDiscount, 161.47m);
            Assert.Equal(cart.SubTotalDiscountWithTax, 180.8464m);
            Assert.Equal(cart.SubTotalWithTax, 1522.6176m);
            Assert.Equal(cart.ShippingSubTotal, 22.00m);
            Assert.Equal(cart.ShippingSubTotalWithTax, 24.64m);
            Assert.Equal(cart.PaymentSubTotal, 44.52m);
            Assert.Equal(cart.PaymentSubTotalWithTax, 49.8624m);
            Assert.Equal(cart.TaxTotal, 150.0072m);
            Assert.Equal(cart.DiscountTotal, 176.47m);
            Assert.Equal(cart.DiscountTotalWithTax, 197.6464m);
            Assert.Equal(cart.FeeTotal, 13.64m);
            Assert.Equal(cart.FeeTotalWithTax, 15.2768m);
            Assert.Equal(cart.FeeWithTax, 14.6832m);
            Assert.Equal(cart.Total, 1413.1772m);
        }
    }
}

