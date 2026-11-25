using System.Linq;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    // VCST-6824: a line total is rounded once from the exact unit price, so the line and the cart total
    // match the unit price shown in the catalog under any currency rounding policy
    public class CartLineTotalsRoundingTests
    {
        [Theory]
        //           RoundingType,  Digits, ListPrice, SalePrice, Tax,    Qty, PlacedPriceWithTax, ExtendedPriceWithTax, CartTotal
        [InlineData("Rounding001",    2,    12.5549,    6.52,     0.20,    1,     7.82,               7.82,                7.82)]
        [InlineData("Rounding001",    2,    12.5649,    6.53,     0.21,    1,     7.90,               7.90,                7.90)]
        [InlineData("Rounding001",    0,    10.50,      7.20,     0.20,    1,     9,                  9,                   9)]
        [InlineData("Rounding001",    0,    10.50,      7.20,     0,       1,     7,                  7,                   7)]
        [InlineData("Rounding005Up",  2,    19.99,     14.97,     0.081,   3,    16.20,              48.55,               48.55)]
        [InlineData("Rounding001",    2,    20.00,     15.00,     0.20,    2,    18.00,              36.00,               36.00)]
        public void CalculateTotals_SingleLine_LineAndCartTotalMatchUnitPrice(string roundingType, int digits, decimal listPrice, decimal salePrice, decimal tax, int quantity,
            decimal expectedPlacedPriceWithTax, decimal expectedExtendedPriceWithTax, decimal expectedCartTotal)
        {
            // Arrange
            var currency = CreateCurrency(roundingType, digits);
            var lineItem = CreateLineItem(listPrice, salePrice, tax, quantity);
            var cart = new ShoppingCart { Currency = currency.Code, Items = [lineItem] };

            // Act
            CreateCalculator(currency).CalculateTotals(cart);

            // Assert
            Assert.Equal(expectedPlacedPriceWithTax, currency.RoundingPolicy.RoundMoney(lineItem.PlacedPriceWithTax, currency));
            Assert.Equal(expectedExtendedPriceWithTax, lineItem.ExtendedPriceWithTax);
            Assert.Equal(expectedCartTotal, cart.Total);
            Assert.Equal(lineItem.ExtendedPrice, lineItem.ListTotal - lineItem.DiscountTotal);
            Assert.Equal(lineItem.ExtendedPriceWithTax, lineItem.ExtendedPrice + lineItem.TaxTotal);
        }

        [Fact]
        public void CalculateTotals_SeveralLines_CartTotalIsSumOfLines()
        {
            // Arrange
            var currency = CreateCurrency("Rounding001", 2);
            var cart = new ShoppingCart
            {
                Currency = currency.Code,
                Items =
                [
                    CreateLineItem(12.5549m, 6.52m, 0.20m, 3),
                    CreateLineItem(20.00m, 15.00m, 0.20m, 2),
                    CreateLineItem(10.0049m, 8.0049m, 0.20m, 5),
                    CreateLineItem(12.5649m, 6.53m, 0.20m, 7),
                ],
            };

            // Act
            CreateCalculator(currency).CalculateTotals(cart);

            // Assert
            Assert.Equal(cart.Items.Sum(x => x.ListTotal), cart.SubTotal);
            Assert.Equal(cart.Items.Sum(x => x.DiscountTotal), cart.SubTotalDiscount);
            Assert.Equal(cart.Items.Sum(x => x.ExtendedPrice), cart.SubTotal - cart.SubTotalDiscount);
            Assert.Equal(cart.Items.Sum(x => x.ExtendedPriceWithTax), cart.Total);
        }

        [Fact]
        public void CalculateTotals_CashRounding_LineIsRoundedToTheCashStep()
        {
            // Arrange
            var currency = CreateCurrency("Rounding005Down", 2);
            var lineItem = CreateLineItem(9.99m, 9.99m, 0m, 3);
            var cart = new ShoppingCart { Currency = currency.Code, Items = [lineItem] };

            // Act
            CreateCalculator(currency).CalculateTotals(cart);

            // Assert: 29.97 -> 29.95
            Assert.Equal(29.95m, lineItem.ExtendedPrice);
            Assert.Equal(29.95m, cart.Total);
        }

        private static LineItem CreateLineItem(decimal listPrice, decimal salePrice, decimal tax, int quantity)
        {
            return new LineItem
            {
                ListPrice = listPrice,
                SalePrice = salePrice,
                DiscountAmount = listPrice - salePrice,
                TaxPercentRate = tax,
                Quantity = quantity,
                Currency = "TST",
                SelectedForCheckout = true,
            };
        }

        private static Currency CreateCurrency(string roundingType, int digits)
        {
            return new Currency(new Language("en-US"), code: "TST")
            {
                DecimalDigits = digits,
                RoundingType = roundingType,
                MidpointRounding = "AwayFromZero",
                RoundingPolicy = new DefaultMoneyRoundingPolicy(),
            };
        }

        private static DefaultShoppingCartTotalsCalculator CreateCalculator(Currency currency)
        {
            var currencyServiceMock = new Mock<ICurrencyService>();
            currencyServiceMock.Setup(x => x.GetAllCurrenciesAsync()).ReturnsAsync([currency]);
            return new DefaultShoppingCartTotalsCalculator(currencyServiceMock.Object);
        }
    }
}
