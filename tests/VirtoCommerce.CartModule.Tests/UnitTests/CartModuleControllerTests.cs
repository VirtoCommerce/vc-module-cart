using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Web.Controllers.Api;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    public class CartModuleControllerTests
    {
        private readonly Mock<IShoppingCartService> _shoppingCartServiceMock = new();
        private readonly Mock<IShoppingCartSearchService> _searchServiceMock = new();
        private readonly Mock<IShoppingCartBuilder> _cartBuilderMock = new();
        private readonly Mock<IShoppingCartTotalsCalculator> _totalsCalculatorMock = new();

        private CartModuleController CreateSut() => new(
            _shoppingCartServiceMock.Object,
            _searchServiceMock.Object,
            _cartBuilderMock.Object,
            _totalsCalculatorMock.Object);

        // The controller calls GetByIdAsync, an extension on top of the interface's GetAsync(IList<string>, string, bool).
        // Moq can only intercept the interface method, so we set up GetAsync.
        private void SetupCart(string cartId, ShoppingCart cart)
        {
            _shoppingCartServiceMock
                .Setup(x => x.GetAsync(It.Is<IList<string>>(ids => ids.Count == 1 && ids[0] == cartId), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(cart == null ? new List<ShoppingCart>() : new List<ShoppingCart> { cart });
        }

        [Fact]
        public async Task GetAvailableShippingRates_ReturnsEmpty_WhenCartIsNull()
        {
            // arrange
            SetupCart("missing-cart", null);

            // act
            var result = await CreateSut().GetAvailableShippingRates("missing-cart");

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Empty((ICollection<ShippingRate>)ok.Value);
            _cartBuilderMock.Verify(x => x.TakeCart(It.IsAny<ShoppingCart>()), Times.Never);
        }

        [Fact]
        public async Task GetAvailableShippingRatesByContext_ReturnsEmpty_WhenContextIsNull()
        {
            // act
            var result = await CreateSut().GetAvailableShippingRatesByContext(null);

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Empty((ICollection<ShippingRate>)ok.Value);
            _cartBuilderMock.Verify(x => x.TakeCart(It.IsAny<ShoppingCart>()), Times.Never);
        }

        [Fact]
        public async Task GetAvailablePaymentMethods_ReturnsEmpty_WhenCartIsNull()
        {
            // arrange
            SetupCart("missing-cart", null);

            // act
            var result = await CreateSut().GetAvailablePaymentMethods("missing-cart");

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Empty((ICollection<PaymentMethod>)ok.Value);
            _cartBuilderMock.Verify(x => x.TakeCart(It.IsAny<ShoppingCart>()), Times.Never);
        }

        [Fact]
        public async Task GetAvailableShippingRates_FlowsThrough_WhenCartIsPresent()
        {
            // arrange
            var cart = new ShoppingCart { Id = "present-cart" };
            var rates = new[] { new ShippingRate() };
            SetupCart("present-cart", cart);
            _cartBuilderMock.Setup(x => x.TakeCart(It.IsAny<ShoppingCart>())).Returns(_cartBuilderMock.Object);
            _cartBuilderMock.Setup(x => x.GetAvailableShippingRatesAsync()).ReturnsAsync(rates);

            // act
            var result = await CreateSut().GetAvailableShippingRates("present-cart");

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(rates, ok.Value);
            _cartBuilderMock.Verify(x => x.TakeCart(It.IsAny<ShoppingCart>()), Times.Once);
        }
    }
}
