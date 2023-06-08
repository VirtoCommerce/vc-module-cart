# Overview

VirtoCommerce.Cart module represents shopping cart management system. This module doesn't have any UI in VC Manager.

VirtoCommerce.Cart module manages customers accumulated list of items, calculates a total for the order, including shipping and handling (i.e., postage and packing) charges and the associated taxes.

![Cart Module](media/screen-cart-module.png)

![Cart Module Info](media/screen-cart-module-info.png)

# Table of contents
- [Features](#features)
- [Concurrency handling](#concurrency-handling)
- [Changing the cart deletion behavior](#changing-the-cart-deletion-behavior)
- [Installation](#installation)
- [Available resources](#available-resources)

## Features

1. Supports multiple carts - if the user is using more than one cart at the same time, all of the carts will be supported by VirtoCommerce.Cart module.

1. Named lists - the user can add the desired products to the  wishlist , which will be saved in the cart. The user can later use the products added to the wishlist to make an order. The wishlist details will be saved by the VirtoCommerce.Cart module.
1. Grouping multiple carts to one order - if the user is using more than one cart, the final order will be created by grouping the details from all the carts filled out by user.
1. Anonymous carts - the user can put products to cart and submit order anonymously, without creating an account.
1. Stock reservation - the selected products will be reserved in stock after adding them to the cart.
1. Multiple payments methods - the user can choose the payment methods he wants before checkout.
1. Create new cart from orders history - the user can add products to a new cart using the previously completed orders (orders history).

The main purpose of the VirtoCommerce.Cart module is to implement customer shopping cart management in VC eCommerce solution. It encapsulates data persistence, management services and exposes REST API endpoints.

The VirtoCommerce.Cart module is connected with the shopping cart API requests:

1. General API request that allows saving the entire shopping cart information on the database;
1. Specific API requests that allow call specific operations, for example delete, edit, choose payment method, choose delivery type, etc.

For more details about the available cart module API, please refer to our [demo](https://virtocommerce.com/request-demo).

The VirtoCommerce.Cart module functionality can be extended and customized based on the specific business needs. In order to extend the cart module functionality, use "Virto Commerce 2.x Cart and Order Module extension" template that can be accessed by following the link below:

https://marketplace.visualstudio.com/items?itemName=Virto-Commerce.VirtoCommerceModuleTemplates

## Concurrency handling

To add the possibility of handling concurrency conflict `CartEntity` contains the concurrency token column named `RowVersion`. If the same data gets modified at the same time EF Core's `SaveChanges()` throws a `DbUpdateConcurrencyException`. In cases when you need to handle such situations you can overrdie the `CommitAsync` method and handle `DbUpdateConcurrencyException`. There's an example of `client-wins` scenario:

```cs
    protected async override Task CommitAsync(IRepository repository)
    {
        bool saveFailed;
        var retry = 0;

        do
        {
            saveFailed = false;

            try
            {
                await repository.UnitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                saveFailed = true;
                retry++;

                if (retry == _commitRetriesCount)
                {
                    throw;
                }

                foreach (var entry in ex.Entries)
                {
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
            }

        } while (saveFailed);
    }
```

## Changing the cart deletion behavior

To change the default cart deletion behavior to better suit your production needs you can register your own extended `IDeleteObsoleteCartsHandler` service from an extension module:

```cs
    public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDeleteObsoleteCartsHandler, CartDeleteHandlerExtended>();
        }
```

An example of card deletion mechanism using raw SQL based on temporary tables:

```cs
public class CartDeleteHandlerExtended : IDeleteObsoleteCartsHandler
    {
        private readonly Func<ICartRepository> _repositoryFactory;

        public CartDeleteHandlerExtended(Func<ICartRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task DeleteObsoleteCarts()
        {
            using (var repository = _repositoryFactory())
            {
                var cartRepository = repository as CartRepository;
                var dbContext = cartRepository.DbContext;

                await dbContext.Database.ExecuteSqlRawAsync(GetDeleteCommand());
            }
        }

        private string GetDeleteCommand()
        {
            var command = @"
                DECLARE @ShoppingCartIds table (ShoppingCartId nvarchar(128))
                DECLARE @ShipmentIds table (ShipmentId nvarchar(128))
                DECLARE @PaymentIds table (PaymentId nvarchar(128))
                DECLARE  @LineItemIds table (LineItemId nvarchar(128))
                Declare @batchSize int = 10000, @tillDate DateTime = DATEADD(MONTH,-6,GETDATE()), @counter int
                SET @counter= 1
                While (@counter<=100)
                BEGIN

                --INSERTION INTO Temporary Tables

                INSERT INTO @ShoppingCartIds (ShoppingCartId)
                SELECT TOP(@batchSize) Id FROM [Cart] WHERE CreatedDate<=@tillDate AND IsDeleted=1 ORDER BY CreatedDate asc;
                INSERT INTO @ShipmentIds (ShipmentId)
                SELECT Id FROM CartShipment WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds);
                INSERT INTO @PaymentIds (PaymentId)
                SELECT Id FROM CartPayment WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds);
                INSERT INTO @LineitemIds (LineItemId)
                SELECT Id FROM CartLineItem WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds);

                --DELETION FROM MAIN Tables

                DELETE FROM CartAddress WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds) 
                        OR ShipmentId IN (SELECT ShipmentId FROM @ShipmentIds)
                        OR PaymentId IN (SELECT PaymentId FROM @PaymentIds);

                DELETE FROM CartDiscount WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds) 
                        OR ShipmentId IN (SELECT ShipmentId FROM @ShipmentIds) 
                        OR LineItemId IN (SELECT LineItemId FROM @LineitemIds) 
                        OR PaymentId IN (SELECT PaymentId FROM @PaymentIds);

                DELETE FROM CartDynamicPropertyObjectValue WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds) 
                        OR ShipmentId IN (SELECT ShipmentId FROM @ShipmentIds) 
                        OR PaymentId IN (SELECT PaymentId FROM @PaymentIds)
                        OR LineItemId IN (SELECT LineItemId FROM @LineitemIds);

                DELETE FROM CartShipmentItem WHERE ShipmentId IN (SELECT ShipmentId FROM @ShipmentIds) 
                        OR LineItemId IN (SELECT LineItemId FROM @LineitemIds);

                DELETE FROM CartTaxDetail WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds) 
                        OR ShipmentId IN (SELECT ShipmentId FROM @ShipmentIds) 
                        OR LineItemId IN (SELECT LineItemId FROM @LineitemIds) 
                        OR PaymentId IN (SELECT PaymentId FROM @PaymentIds);

                DELETE FROM CartCoupon WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds);
                DELETE FROM CartShipment WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds);
                DELETE FROM CartPayment WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds);
                DELETE FROM CartLineItem WHERE ShoppingCartId IN (SELECT ShoppingCartId FROM @ShoppingCartIds);
                DELETE FROM Cart WHERE Id IN (SELECT ShoppingCartId FROM @ShoppingCartIds);

                --DELETION FROM TEMPORARY TABLES

                DELETE FROM @ShoppingCartIds;
                DELETE FROM @ShipmentIds;
                DELETE FROM @LineitemIds;
                DELETE FROM @PaymentIds;

                SET @counter =@counter + 1;
                END";

            return command;
        }
    }

```

## Installation
Installing the module:
* Automatically: in VC Manager go to More -> Modules -> Shopping cart module -> Install
* Manually: download module zip package from https://github.com/VirtoCommerce/vc-module-cart/releases. In VC Manager go to More -> Modules -> Advanced -> upload module package -> Install.

## Available resources
* Module related service implementations as a <a href="https://www.nuget.org/packages/VirtoCommerce.CartModule.Data" target="_blank">NuGet package</a>
