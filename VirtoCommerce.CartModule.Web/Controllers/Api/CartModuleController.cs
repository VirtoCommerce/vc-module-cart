using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.CartModule.Web.Controllers.Api
{
    [RoutePrefix("api/cart")]
    [CheckPermission(Permission = PredefinedPermissions.Query)]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CartModuleController : ApiController
    {
        private readonly ICustomerOrderBuilder _customerOrderBuilder;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartSearchService _searchService;
        private readonly IShoppingCartBuilder _cartBuilder;

        public CartModuleController(IShoppingCartService shoppingCartService, IShoppingCartSearchService searchService, IShoppingCartBuilder cartBuilder, ICustomerOrderBuilder customerOrderBuilder)
        {
            _shoppingCartService = shoppingCartService;
            _searchService = searchService;
            _cartBuilder = cartBuilder;
            _customerOrderBuilder = customerOrderBuilder;
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/current")]
        [ResponseType(typeof(ShoppingCart))]
        public IHttpActionResult GetCart(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName)
                        .EvaluateTaxes()
                        .EvaluatePromotions();

            return Ok(_cartBuilder.Cart);
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/itemscount")]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetCartItemsCount(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);
            return Ok(_cartBuilder.Cart.Items.Count);
        }
     

        [HttpPost]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/items")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddItemToCart(string storeId, string customerId, string cartName, string currency, string cultureName, [FromBody] LineItem lineItem)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddItem(lineItem).Save();
            }
            return Ok();
        }

        [HttpPut]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/items")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ChangeCartItem(string storeId, string customerId, string cartName, string currency, string cultureName, string lineItemId, int quantity)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                var lineItem = _cartBuilder.Cart.Items.FirstOrDefault(i => i.Id == lineItemId);
                if (lineItem != null)
                {
                    _cartBuilder.ChangeItemQuantity(lineItemId, quantity).Save();
                }
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/items")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> RemoveCartItem(string storeId, string customerId, string cartName, string currency, string cultureName, string lineItemId)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.RemoveItem(lineItemId).Save();
            }

            return Ok(_cartBuilder.Cart.Items.Count);
        }

        [HttpPost]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/clear")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ClearCart(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.Clear().Save();
            }

            return Ok();
        }

        [HttpPost]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/mergewith/{cartId}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> MergeWithCart(string storeId, string customerId, string cartName, string currency, string cultureName, string cartId)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                var cart = _shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault();                
                _cartBuilder.MergeWithCart(cart);
                _cartBuilder.Save();
            }
            return Ok();
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/shipments/{shipmentId}/shippingmethods")]
        [ResponseType(typeof(ICollection<ShippingRate>))]
        public IHttpActionResult GetAvailableShippingRates(string storeId, string customerId, string cartName, string currency, string cultureName, string shipmentId)
        {
            var shippingRates = _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName)
                .GetAvailableShippingRates();

            return Ok(shippingRates);
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/paymentmethods")]
        [ResponseType(typeof(ICollection<Domain.Payment.Model.PaymentMethod>))]
        public IHttpActionResult GetAvailablePaymentMethods(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            var paymentMethods = _cartBuilder.GetAvailablePaymentMethods();

            return Ok(paymentMethods);
        }

        [HttpPost]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/coupons/{couponCode}")]
        [ResponseType(typeof(Coupon))]
        public async Task<IHttpActionResult> AddCartCoupon(string storeId, string customerId, string cartName, string currency, string cultureName, string couponCode)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddCoupon(couponCode);
                _cartBuilder.Save();
            }

            return Ok(_cartBuilder.Cart.Coupon);
        }

        [HttpDelete]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/coupons")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> RemoveCartCoupon(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.RemoveCoupon();
                _cartBuilder.Save();
            }

            return Ok();
        }

        [HttpPost]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/shipments")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddOrUpdateCartShipment(string storeId, string customerId, string cartName, string currency, string cultureName, [FromBody] Shipment shipment)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddOrUpdateShipment(shipment).Save();
            }

            return Ok();
        }

        [HttpPost]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/payments")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddOrUpdateCartPayment(string storeId, string customerId, string cartName, string currency, string cultureName, [FromBody] Payment payment)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddOrUpdatePayment(payment).Save();
            }

            return Ok();
        }

        [HttpPost]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/createorder")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> PlaceOrder(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                var customerOrder = _customerOrderBuilder.PlaceCustomerOrder(_cartBuilder);
                return Ok(customerOrder.Id);
            }
        }

        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <param name="id">Shopping cart id</param>
        /// <response code="200"></response>
        /// <response code="404">Shopping cart not found</response>
        [HttpGet]
        [Route("carts/{id}")]
        [ResponseType(typeof(ShoppingCart))]
        public IHttpActionResult GetCartById(string id)
        {
            var retVal = _shoppingCartService.GetByIds(new[] { id });
            if (retVal == null)
            {
                return NotFound();
            }
            return Ok(retVal);
        }

      
        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        /// <response code="204">Operation completed</response>
        [HttpPost]
        [Route("carts")]
        [ResponseType(typeof(ShoppingCart))]
        [CheckPermission(Permission = PredefinedPermissions.Create)]
        public IHttpActionResult Create(ShoppingCart cart)
        {
            _shoppingCartService.SaveChanges(new[] { cart });
            return Ok(cart);
        }

        /// <summary>
        /// Update shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        [HttpPut]
        [Route("carts")]
        [ResponseType(typeof(ShoppingCart))]
        [CheckPermission(Permission = PredefinedPermissions.Update)]
        public IHttpActionResult Update(ShoppingCart cart)
        {
            _shoppingCartService.SaveChanges(new[] { cart });
            var retVal = _shoppingCartService.GetByIds(new[] { cart.Id });
            return Ok(retVal);
        }
                  

        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <response code="204">Operation completed</response>
        [HttpDelete]
        [Route("carts")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.Delete)]
        public IHttpActionResult DeleteCarts([FromUri] string[] ids)
        {
            _shoppingCartService.Delete(ids);
            return Ok();
        }
              
        private static string GetAsyncLockCartKey(string cartId)
        {
            return "Cart:" + cartId;
        }

    }
}
