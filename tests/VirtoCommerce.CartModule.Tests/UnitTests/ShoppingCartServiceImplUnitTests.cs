using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    public class ShoppingCartServiceImplUnitTests
    {
        private readonly Mock<IShoppingCartTotalsCalculator> _calculatorMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartServiceImplUnitTests()
        {
            _calculatorMock = new Mock<IShoppingCartTotalsCalculator>();
            _cartRepositoryMock = new Mock<ICartRepository>();
            _repositoryFactory = () => _cartRepositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _cartRepositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _eventPublisherMock = new Mock<IEventPublisher>();
            var memoryCache = GetPlatformMemoryCache();

            _shoppingCartService = new ShoppingCartService(
                _repositoryFactory, _calculatorMock.Object,
                _eventPublisherMock.Object, memoryCache);
        }

        [Fact]
        public async Task GetByIdsAsync_ReturnCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var cartIds = new[] { cartId };
            var list = new List<ShoppingCartEntity> { new ShoppingCartEntity { Id = cartId } };
            _cartRepositoryMock.Setup(x => x.GetShoppingCartsByIdsAsync(cartIds, null))
                .ReturnsAsync(list.ToArray());

            //Act
            var result = await _shoppingCartService.GetByIdsAsync(new[] { cartId });

            //Assert
            Assert.True(result.Any());
            Assert.Contains(result, cart => cart.Id.Equals(cartId));
        }

        [Fact]
        public async Task SaveChangesAsync_CreateCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var entity = new ShoppingCartEntity { Id = cartId };
            var carts = new List<ShoppingCart> { entity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()) };

            //Act
            await _shoppingCartService.SaveChangesAsync(carts.ToArray());

            //Assert
        }

        [Fact]
        public async Task SaveChangesAsync_SaveCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var cartIds = new[] { cartId };
            var entity = new ShoppingCartEntity { Id = cartId };
            var list = new List<ShoppingCartEntity> { entity };
            _cartRepositoryMock.Setup(n => n.GetShoppingCartsByIdsAsync(cartIds, null))
                .ReturnsAsync(list.ToArray());
            var carts = new List<ShoppingCart> { entity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()) };

            //Act
            await _shoppingCartService.SaveChangesAsync(carts.ToArray());

            //Assert
        }

        private IPlatformMemoryCache GetPlatformMemoryCache()
        {
            var provider = new ServiceCollection().AddMemoryCache().BuildServiceProvider();
            var memoryCache = provider.GetService<IMemoryCache>();
            var mockLog = new Mock<ILogger<PlatformMemoryCache>>();

            return new PlatformMemoryCache(memoryCache, Options.Create(new CachingOptions()), mockLog.Object);
        }
    }
}
