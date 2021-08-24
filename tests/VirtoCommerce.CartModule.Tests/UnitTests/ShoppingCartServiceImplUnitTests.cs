using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Data.Validation;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.CartModule.Test.UnitTests
{
    public class ShoppingCartServiceImplUnitTests
    {
        private readonly Mock<IShoppingCartTotalsCalculator> _calculatorMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<ICacheEntry> _cacheEntryMock;

        public ShoppingCartServiceImplUnitTests()
        {
            _calculatorMock = new Mock<IShoppingCartTotalsCalculator>();
            _cartRepositoryMock = new Mock<ICartRepository>();
            _repositoryFactory = () => _cartRepositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _cartRepositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _eventPublisherMock = new Mock<IEventPublisher>();
            _cacheEntryMock = new Mock<ICacheEntry>();
            _cacheEntryMock.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            FluentValidation.ValidatorOptions.LanguageManager.Enabled = false;
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
            var platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            platformMemoryCacheMock.Setup(x => x.GetDefaultCacheEntryOptions()).Returns(() => new MemoryCacheEntryOptions());
            var service = GetShoppingCartService(platformMemoryCacheMock.Object);
            var cacheKey = CacheKey.With(service.GetType(), nameof(service.GetByIdsAsync), string.Join("-", cartIds), null);
            platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_cacheEntryMock.Object);

            //Act
            var result = await service.GetByIdsAsync(new[] { cartId });

            //Assert
            Assert.True(result.Any());
            Assert.Contains(result, cart => cart.Id.Equals(cartId));
        }

        [Fact]
        public async Task SaveChangesAsync_CreateCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var entity = new ShoppingCartEntity { Id = cartId, StoreId = "StoreId", CustomerId = "CustomerId", Currency = "USD" };
            var carts = new List<ShoppingCart> { entity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()) };
            var service = GetShoppingCartService();

            //Act
            await service.SaveChangesAsync(carts.ToArray());

            //Assert
        }

        [Fact]
        public async Task SaveChangesAsync_SaveCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var cartIds = new[] { cartId };
            var entity = new ShoppingCartEntity { Id = cartId, StoreId = "StoreId", CustomerId = "CustomerId", Currency = "USD" };
            var list = new List<ShoppingCartEntity> { entity };
            _cartRepositoryMock.Setup(n => n.GetShoppingCartsByIdsAsync(cartIds, null))
                .ReturnsAsync(list.ToArray());
            var carts = new List<ShoppingCart> { entity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()) };
            var service = GetShoppingCartService();

            //Act
            await service.SaveChangesAsync(carts.ToArray());

            //Assert
        }

        [Fact]
        public async Task GetByIdsAsync_GetThenSaveCart_ReturnCachedCart()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var newCart = new ShoppingCart { Id = id, StoreId = "StoreId", CustomerId = "CustomerId", Currency = "USD" };
            var newCartEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance().FromModel(newCart, new PrimaryKeyResolvingMap());
            var service = GetCustomerOrderServiceWithPlatformMemoryCache();
            _cartRepositoryMock.Setup(x => x.Add(newCartEntity))
                .Callback(() =>
                {
                    _cartRepositoryMock.Setup(o => o.GetShoppingCartsByIdsAsync(new[] { id }, null))
                        .ReturnsAsync(new[] { newCartEntity });
                });

            //Act
            var nullCart = await service.GetByIdAsync(id);
            await service.SaveChangesAsync(new[] { newCart });
            var Cart = await service.GetByIdAsync(id);

            //Assert
            Assert.NotEqual(nullCart, Cart);
        }

        [Fact]
        public async Task SaveChangesAsync_ValidateNull()
        {
            //Arrange
            var service = GetShoppingCartService();

            //Act
            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => service.SaveChangesAsync(null));

            //Assert
            Assert.EndsWith(ShoppingCartsValidator.CartsNotSuppliedMessage, ex.Message);
        }

        [Fact]
        public async Task SaveChangesAsync_ValidateCurrency()
        {
            //Arrange
            var entity = new ShoppingCart { Id = "id", StoreId = "StoreId", CustomerId = "CustomerId", Currency = null };
            var carts = new[] { entity };
            var service = GetShoppingCartService();

            //Act
            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => service.SaveChangesAsync(carts));

            //Assert
            Assert.EndsWith("'Currency' must not be empty.", ex.Message);
        }

        [Fact]
        public async Task SaveChangesAsync_ValidateBillingAddress()
        {
            //Arrange
            var entity = new ShoppingCart { Id = "id", StoreId = "StoreId", CustomerId = "CustomerId", Currency = "USD" };
            entity.Payments = new[] { new Payment { Currency = "USD", BillingAddress = new Address { LastName = null } } };
            var carts = new[] { entity };
            var service = GetShoppingCartService();

            //Act
            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => service.SaveChangesAsync(carts));

            //Assert
            Assert.EndsWith("'Last Name' must not be empty.", ex.Message);
        }

        private ShoppingCartService GetCustomerOrderServiceWithPlatformMemoryCache()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var platformMemoryCache = new PlatformMemoryCache(memoryCache, Options.Create(new CachingOptions()), new Mock<ILogger<PlatformMemoryCache>>().Object);
            _cartRepositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);

            return GetShoppingCartService(platformMemoryCache);
        }

        private ShoppingCartService GetShoppingCartService()
        {
            var platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            return GetShoppingCartService(platformMemoryCacheMock.Object);
        }

        private ShoppingCartService GetShoppingCartService(IPlatformMemoryCache platformMemoryCache)
        {
            return new ShoppingCartService(
                _repositoryFactory, _calculatorMock.Object,
                _eventPublisherMock.Object, platformMemoryCache);
        }
    }
}
