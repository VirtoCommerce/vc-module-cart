using System;
using System.Collections.Generic;
using System.Linq;
using CacheManager.Core;
using Moq;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Data.Builders;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CoreModule.Data.Payment;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.CoreModule.Data.Services;
using VirtoCommerce.CoreModule.Data.Shipping;
using VirtoCommerce.CoreModule.Data.Tax;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Domain.Tax.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.MarketingModule.Data.Services;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.DynamicProperties;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Data.Serialization;
using Xunit;

namespace VirtoCommerce.CartModule.Test
{
	[Trait("Category", "CI")]
	public class CartBuilderTests
	{
		private const string StoreId = "Clothing";
		private const string CartName = "Default";
		private const string CurrencyCode = "USD";
		private const string LanguageCultureName = "en-US";
		private const string CouponCode = "Code";

		[Fact]
		public void GetOrCreateNewTransientCartTest()
		{
			//Act
			var builder = GetCartBuilder();

			//Assert
			Assert.NotNull(builder.Cart);
		}

		[Fact]
		public void AddItemTest()
		{
			//Assign
			var builder = GetCartBuilder();

			//Act
			builder.AddItem(GetAddItemModel()).Save();

			//Assert
			Assert.Equal(builder.Cart.Items.Count, 1);
		}

		[Fact]
		public void ChangeItemQuantityTest()
		{
			//Assign
			var builder = GetCartBuilder();
			builder.AddItem(GetAddItemModel()).Save();

			//Act
			builder.ChangeItemQuantity(builder.Cart.Items.First().Id, 2).Save();

			//Assert
			Assert.Equal(builder.Cart.Items.First().Quantity, 2);
		}

		[Fact]
		public void RemoveItemTest()
		{
			//Assign
			var builder = GetCartBuilder();
			builder.AddItem(GetAddItemModel()).Save();

			//Act
			builder.RemoveItem(builder.Cart.Items.First().Id).Save();

			//Assert
			Assert.Equal(builder.Cart.Items.Count, 0);
		}

		[Fact]
		public void ClearTest()
		{
			//Assign
			var builder = GetCartBuilder();
			builder.AddItem(GetAddItemModel()).Save();

			//Act
			builder.Clear().Save();

			//Assert
			Assert.Equal(builder.Cart.Items.Count, 0);
		}

		[Fact]
		public void AddCouponTest()
		{
			//Assign
			var builder = GetCartBuilder();

			//Act
			builder.AddCoupon(CouponCode).Save();

			//Assert
			Assert.NotNull(builder.Cart.Coupon);
		}

		[Fact]
		public void RemoveCouponTest()
		{
			//Assign
			var builder = GetCartBuilder();
			builder.AddCoupon(CouponCode).Save();

			//Act
			builder.RemoveCoupon().Save();

			//Assert
			Assert.Null(builder.Cart.Coupon);
		}

		[Fact]
		public void GetAvailableShippingMethodsTest()
		{
			//Assign
			var builder = GetCartBuilder();

			//Act
			var shippingRates = builder.GetAvailableShippingRates();

			//Assert
			Assert.True(shippingRates.Count > 0);
		}

		[Fact]
		public void AddOrUpdateShipmentTest()
		{
			//Assign
			var builder = GetCartBuilder();

			//Act
			var updateModel = new ShipmentUpdateModel()
			{
				ShipmentMethodCode = "FixedRate",
				ShipmentMethodOption = "Air",
				ShippingPrice = 0
			};
			builder.AddOrUpdateShipment(updateModel).Save();

			//Assert
			Assert.True(builder.Cart.Shipments.Count == 1);
		}

		[Fact]
		public void GetAvailablePaymentMethodsTest()
		{
			//Assign
			var builder = GetCartBuilder();

			//Act
			var paymentMethods = builder.GetAvailablePaymentMethods();

			//Assert
			Assert.True(paymentMethods.Count > 0);
		}

		[Fact]
		public void AddOrUpdatePaymentTest()
		{
			//Assign
			var builder = GetCartBuilder();

			//Act
			var updateModel = new PaymentUpdateModel()
			{
				PaymentGatewayCode = "DefaultManualPaymentMethod",
				Amount = 100
			};
			builder.AddOrUpdatePayment(updateModel).Save();

			//Assert
			Assert.True(builder.Cart.Payments.Count > 0);
		}

		private AddItemModel GetAddItemModel()
		{
			var addItemModel = new AddItemModel()
			{
				ProductId = Guid.NewGuid().ToString(),
				CatalogId = Guid.NewGuid().ToString(),
				Sku = Guid.NewGuid().ToString(),
				Name = Guid.NewGuid().ToString(),
				Quantity = 1,
				ListPrice = 10,
				SalePrice = 10,
				ExtendedPrice = 10,
				PlacedPrice = 10,
				DiscountTotal = 5,
				TaxTotal = 5
			};
			return addItemModel;
		}

		private ICartBuilder GetCartBuilder()
		{
			var storeService = GetStoreService();
			var shoppingCartService = GetShoppingCartService();
			var shoppingCartSearchService = GetShoppingCartSearchService();
			var marketingPromoEvaluator = GetMarketingPromoEvaluator();
			var cacheManager = GetCacheManager();

			var builder = new CartBuilder(storeService, shoppingCartService, shoppingCartSearchService, marketingPromoEvaluator, cacheManager, null, null);

			builder.GetOrCreateNewTransientCart(StoreId, Guid.NewGuid().ToString(), CartName, CurrencyCode, LanguageCultureName).Save();

			return builder;
		}

		private ICacheManager<object> GetCacheManager()
		{
			return new Mock<ICacheManager<object>>().Object;
		}

		private IPromotionService GetPromotionService()
		{
			Func<IMarketingRepository> repository = () => new MarketingRepositoryImpl("VirtoCommerce", new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));

			var promotionExtensionManager = new DefaultMarketingExtensionManagerImpl();

			return new PromotionServiceImpl(repository, promotionExtensionManager, GetExpressionSerializer(), GetCacheManager());
		}

		private IExpressionSerializer GetExpressionSerializer()
		{
			return new XmlExpressionSerializer();
		}

		private IMarketingPromoEvaluator GetMarketingPromoEvaluator()
		{
			return new DefaultPromotionEvaluatorImpl(GetPromotionService(), GetCacheManager());
		}

		private IShoppingCartSearchService GetShoppingCartSearchService()
		{
			Func<ICartRepository> repositoryFactory = () => new CartRepositoryImpl("VirtoCommerce", new AuditableInterceptor(null), new EntityPrimaryKeyGeneratorInterceptor());
			return new ShoppingCartSearchServiceImpl(repositoryFactory);
		}

		private IShoppingCartService GetShoppingCartService()
		{
			Func<ICartRepository> repositoryFactory = () => new CartRepositoryImpl("VirtoCommerce", new AuditableInterceptor(null), new EntityPrimaryKeyGeneratorInterceptor());
			return new ShoppingCartServiceImpl(repositoryFactory, new Mock<IEventPublisher<CartChangeEvent>>().Object, new Mock<IItemService>().Object, GetDynamicPropertyService());
		}

		private ICommerceService GetCommerceService()
		{
			Func<IСommerceRepository> repositoryFactory = () => new CommerceRepositoryImpl("VirtoCommerce", new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));
			return new CommerceServiceImpl(repositoryFactory);
		}

		private IDynamicPropertyService GetDynamicPropertyService()
		{
			Func<IPlatformRepository> platformRepositoryFactory = () => new PlatformRepository("VirtoCommerce", new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));
			return new DynamicPropertyService(platformRepositoryFactory);
		}

		private IStoreService GetStoreService()
		{
			//Func<IStoreRepository> repositoryFactory = () => new StoreRepositoryImpl("VirtoCommerce", new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));
			//return new StoreServiceImpl(repositoryFactory, GetCommerceService(), null, GetDynamicPropertyService(), null, null, null, null);
			var storeServiceMock = new Mock<IStoreService>();
			var storeMock = new Store()
			{
				Id = StoreId,
				TaxProviders = new List<TaxProvider>() { new FixedTaxRateProvider() },
				ShippingMethods = new List<ShippingMethod>() { new FixedRateShippingMethod(new SettingEntry[] { }) },
				PaymentMethods = new List<PaymentMethod>()
				{
					new DefaultManualPaymentMethod()
					{
						IsActive = true
					}
				}
			};

			storeServiceMock.Setup(s => s.GetById(It.IsAny<string>())).Returns(storeMock);
			return storeServiceMock.Object;
		}
	}
}
