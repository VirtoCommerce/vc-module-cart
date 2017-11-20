using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Repositories;
using Xunit;

namespace VirtoCommerce.CartModule.Test
{
    public class CRUDScenarios 
    {
        [Fact]
        public void CreateNewFullGraphCart()
        {
            var cart = GetTestCart("cart"); // + Guid.NewGuid().ToString()
            var cartService = GetCartService();

            cartService.SaveChanges(new[] { cart });
            cart = cartService.GetByIds(new[] { cart.Id }).First();


            Assert.NotNull(cart);
        }

     

        private static ShoppingCart  GetTestCart(string id)
        {
            var order = new ShoppingCart
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
            order.Items = new List<LineItem>();
            order.Items.Add(item1);
            order.Items.Add(item2);

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
            foreach(var lineItem in order.Items)
            {
                var shipmentItem = new ShipmentItem
                {
                     LineItem = lineItem,
                     Quantity = lineItem.Quantity
                };
                shipment.Items.Add(shipmentItem);
            }

            order.Shipments = new List<Shipment>();
            order.Shipments.Add(shipment);

       
            return order;
        }

        private static Func<ICartRepository> GetCartRepositoryFactory()
        {
            Func<ICartRepository> cartRepositoryFactory = () =>
            {
                return new CartRepositoryImpl("VirtoCommerce",
                    new AuditableInterceptor(null),
                    new EntityPrimaryKeyGeneratorInterceptor());
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
