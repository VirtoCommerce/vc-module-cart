using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Repositories;
using Xunit;

namespace VirtoCommerce.CartModule.Test
{
    [Trait("Category", "Integration")]
    public class CartPersistenceTests
    {
        [Fact]
        public void CreateNewFullGraphCart()
        {
            var cart = GetTestCart(Guid.NewGuid().ToString());
            var cartService = GetCartService();

            cartService.SaveChanges(new[] { cart });
            cart = cartService.GetByIds(new[] { cart.Id }).First();

            Assert.NotNull(cart);
            Assert.Single(cart.Payments);
            Assert.Single(cart.Shipments);
            Assert.True(cart.Shipments.Single().Items.Count() == 2);            
        }

        [Fact]
        public void AddNewShipmentItemsForExistCart()
        {
            var cart = GetTestCart(Guid.NewGuid().ToString());
            var cartService = GetCartService();

            cartService.SaveChanges(new[] { cart });
            cart = cartService.GetByIds(new[] { cart.Id }).First();

            //On the addition of new shipping items that referenced to existing line items
            //With set ShipmentItem.LineItemId property
            cart.Shipments.First().Items.Add(new ShipmentItem { LineItemId = cart.Items.First().Id , Quantity = 1 });
            //With set ShipmentItem.LineItem property
            cart.Shipments.First().Items.Add(new ShipmentItem { LineItem = new LineItem() { Id = cart.Items.First().Id }, Quantity = 100 });
            cartService.SaveChanges(new[] { cart });

            cart = cartService.GetByIds(new[] { cart.Id }).First();

            Assert.True(cart.Shipments.Single().Items.Count() == 4);

        }

     

        private static ShoppingCart  GetTestCart(string id)
        {
            var result = new ShoppingCart
            {
                Id = id,
                Currency = "USD",
                CustomerId = "vasja customer",
                StoreId = "test store",
                Addresses = new[]
                {
                            new Address {
                            AddressType = AddressType.Shipping,
                            City = "london",
                            Phone = "+68787687",
                            PostalCode = "22222",
                            CountryCode = "ENG",
                            CountryName = "England",
                            Email = "user@mail.com",
                            FirstName = "first name",
                            LastName = "last name",
                            Line1 = "line 1",
                            Organization = "org1"
                            }
                        }.ToList(),
                Discounts = new[] {
                    new Discount
                {
                    PromotionId = "testPromotion",
                    Currency = "USD",
                    DiscountAmount = 12,
                    Coupon = "ssss"
                    }
                }
            };
            var item1 = new LineItem
            {
                Sku = "shoes",
                SalePrice = 10,
                ListPrice = 10,
                ProductId = "shoes",
                CatalogId = "catalog",
                Currency = "USD",
                CategoryId = "category",
                Name = "shoes",
                Quantity = 2,
                FulfillmentLocationCode = "warehouse1",
                Discounts = new[] {  new Discount
                {
                    PromotionId = "itemPromotion",
                    Currency = "USD",
                    DiscountAmount = 12,
                    Coupon =  "ssss"
                }}
            };
            var item2 = new LineItem
            {
                Sku = "t-shirt",
                SalePrice = 10,
                ListPrice = 10,
                ProductId = "t-shirt",
                CatalogId = "catalog",
                CategoryId = "category",
                Currency = "USD",
                Name = "t-shirt",
                Quantity = 2,
                FulfillmentLocationCode = "warehouse1",
            };
            result.Items = new List<LineItem>();
            result.Items.Add(item1);
            result.Items.Add(item2);

            var shipment = new Shipment
            {
                Currency = "USD",
                DeliveryAddress = new Address
                {
                    City = "london",
                    CountryName = "England",
                    Phone = "+68787687",
                    PostalCode = "2222",
                    CountryCode = "ENG",
                    Email = "user@mail.com",
                    FirstName = "first name",
                    LastName = "last name",
                    Line1 = "line 1",
                    Organization = "org1"
                },
                Discounts = new[] {  new Discount
                {
                    PromotionId = "testPromotion",
                    Currency = "USD",
                    DiscountAmount = 12,
                    Coupon = ""
                }},

            };

            shipment.Items = new List<ShipmentItem>();
            foreach(var lineItem in result.Items)
            {
                var shipmentItem = new ShipmentItem
                {
                     LineItem = lineItem,
                     Quantity = lineItem.Quantity
                };
                shipment.Items.Add(shipmentItem);
            }

            result.Shipments = new List<Shipment>();
            result.Shipments.Add(shipment);

            var payment = new Payment
            {
                Amount = 100,
                BillingAddress = result.Addresses.First(),
                Currency = "USD",
                Discounts = new[] {  new Discount
                {
                    PromotionId = "testPromotion",
                    Currency = "USD",
                    DiscountAmount = 12,
                    Coupon = ""
                }},
                TaxPercentRate = 0.2m,
                PaymentGatewayCode = "FeDex",
                OuterId = "123"
            };
            result.Payments = new List<Payment>();
            result.Payments.Add(payment);

            return result;
        }

        private static Func<ICartRepository> GetCartRepositoryFactory()
        {
            Func<ICartRepository> cartRepositoryFactory = () =>
            {
                var connectionString = ConfigurationHelper.GetAppSettingsValue("VC_DATABASE", "VirtoCommerce");
                return new CartRepositoryImpl(connectionString, new AuditableInterceptor(null), new EntityPrimaryKeyGeneratorInterceptor());
            };
            return cartRepositoryFactory;
        }

        private static ShoppingCartServiceImpl GetCartService()
        {
            Func<IPlatformRepository> platformRepositoryFactory = () => new PlatformRepository("VirtoCommerce", new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));

            //var dynamicPropertyService = new DynamicPropertyService(platformRepositoryFactory);
            var dynamicPropertyService = new Mock<IDynamicPropertyService>().Object;
            var changedEventPublisher = new EventPublisher<CartChangedEvent>(Enumerable.Empty<IObserver<CartChangedEvent>>().ToArray());
            var changingEventPublisher = new EventPublisher<CartChangeEvent>(Enumerable.Empty<IObserver<CartChangeEvent>>().ToArray());

            var orderService = new ShoppingCartServiceImpl(GetCartRepositoryFactory(), changingEventPublisher, dynamicPropertyService, changedEventPublisher);
            
            return orderService;
        }
    }
}
