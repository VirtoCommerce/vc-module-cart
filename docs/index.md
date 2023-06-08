# Overview

The VirtoCommerce.Cart module is responsible for managing the shopping cart system. It provides functionality to manage a customer's accumulated list of items, calculate the total for the order (including shipping, handling, and taxes), and handle various cart-related operations.

Please note that the VirtoCommerce.Cart module does not have any user interface in the VC Manager. It primarily serves as the backend system for shopping cart management.

## Key Features

* **Supports Multiple Carts**: The module supports multiple carts, allowing users to work with more than one cart simultaneously.
* **Named Lists**: Users can create wishlists and add desired products to them. These wishlists are saved in the cart and can be used later to place an order. The VirtoCommerce.Cart module manages the details of the wishlist.
* **Grouping Multiple Carts**: If a user is using multiple carts, the VirtoCommerce.Cart module can group the details from all the carts and create a final order.
* **Anonymous Carts**: Users have the option to put products in a cart and submit an order anonymously, without creating an account.
* **Stock Reservation**: The module supports stock reservation. When products are added to the cart, they are reserved in the stock to ensure availability during the checkout process.
* **Multiple Payment Methods**: Users can choose from multiple payment methods before checkout, providing flexibility in the payment process.
* **Create Cart from Order History**: Users can create a new cart by adding products from their order history, making it convenient to reorder previously purchased items.
* **Soft Delete**: Performs a soft delete on their shopping carts in the Virto Commerce system. Instead of permanently deleting the cart, this feature provides the option to mark the cart as deleted without completely removing it from the system.

## API Documentation

The VirtoCommerce.Cart module provides a REST API to interact with the shopping cart functionality.

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

