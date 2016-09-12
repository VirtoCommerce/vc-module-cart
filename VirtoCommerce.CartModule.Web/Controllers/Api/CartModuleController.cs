using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using CacheManager.Core;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.CartModule.Web.Controllers.Api
{
    [RoutePrefix("api/carts")]
    [CheckPermission(Permission = PredefinedPermissions.Query)]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CartModuleController : ApiController
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartSearchService _searchService;
        private readonly IShoppingCartBuilder _cartBuilder;
        private readonly ICacheManager<object> _cacheManager;

        public CartModuleController(IShoppingCartService shoppingCartService, IShoppingCartSearchService searchService, 
                                    IShoppingCartBuilder cartBuilder, ICacheManager<object> cacheManager)
        {
            _shoppingCartService = shoppingCartService;
            _searchService = searchService;
            _cartBuilder = cartBuilder;
            _cacheManager = cacheManager;
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/{cartName}/{currency}/{cultureName}/current")]
        [ResponseType(typeof(ShoppingCart))]
        public async Task<IHttpActionResult> GetCart(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(string.Join(":", storeId, customerId, cartName, currency))).LockAsync())
            {
                _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName)
                               .EvaluatePromotions()
                               .EvaluateTaxes();

            }
            return Ok(_cartBuilder.Cart);
            }

        [HttpGet]
        [Route("{cartId}/itemscount")]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetCartItemsCount(string cartId)
            {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            return Ok(_cartBuilder.Cart.Items.Count);
        }


        [HttpPost]
        [Route("{cartId}/items")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddItemToCart(string cartId, [FromBody] LineItem lineItem)
            {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddItem(lineItem).Save();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [Route("{cartId}/items")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ChangeCartItem(string cartId, string lineItemId, int quantity)
            {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                var lineItem = _cartBuilder.Cart.Items.FirstOrDefault(i => i.Id == lineItemId);
                if (lineItem != null)
                {
                    _cartBuilder.ChangeItemQuantity(lineItemId, quantity).Save();
            }
        }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{cartId}/items/{lineItemId}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> RemoveCartItem(string cartId, string lineItemId)
        {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.RemoveItem(lineItemId).Save();
            }
            return Ok(_cartBuilder.Cart.Items.Count);
        }

        [HttpDelete]
        [Route("{cartId}/items")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ClearCart(string cartId)
        {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.Clear().Save();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPatch]
        [Route("{cartId}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> MergeWithCart(string cartId, [FromBody]ShoppingCart otherCart)
        {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.MergeWithCart(otherCart).Save();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("{cartId}/availshippingrates")]
        [ResponseType(typeof(ICollection<ShippingRate>))]
        public IHttpActionResult GetAvailableShippingRates(string cartId)
        {
            var shippingRates = _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault())
                                            .GetAvailableShippingRates();
            return Ok(shippingRates);
        }

        [HttpGet]
        [Route("{cartId}/availpaymentmethods")]
        [ResponseType(typeof(ICollection<Domain.Payment.Model.PaymentMethod>))]
        public IHttpActionResult GetAvailablePaymentMethods(string cartId)
        {
            var paymentMethods = _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault())
                                             .GetAvailablePaymentMethods();
            return Ok(paymentMethods);
        }

        [HttpPost]
        [Route("{cartId}/coupons/{couponCode}")]
        [ResponseType(typeof(Coupon))]
        public async Task<IHttpActionResult> AddCartCoupon(string cartId, string couponCode)
        {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddCoupon(couponCode).Save();
            }
            return Ok(_cartBuilder.Cart.Coupon);
        }

        [HttpDelete]
        [Route("{cartId}/coupons/{couponCode}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> RemoveCartCoupon(string cartId, string couponCode)
        {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.RemoveCoupon().Save();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{cartId}/shipments")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddOrUpdateCartShipment(string cartId, [FromBody] Shipment shipment)
        {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddOrUpdateShipment(shipment).Save();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{cartId}/payments")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddOrUpdateCartPayment(string cartId, [FromBody] Payment payment)
        {
            _cartBuilder.TakeCart(_shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault());

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            {
                _cartBuilder.AddOrUpdatePayment(payment).Save();
        }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <param name="cartId">Shopping cart id</param>
        [HttpGet]
        [Route("{cartId}")]
        [ResponseType(typeof(ShoppingCart))]
        public IHttpActionResult GetCartById(string cartId)
        {
            var retVal = _shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault();
            return Ok(retVal);
        }


        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        [HttpPost]
        [Route("")]
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
        [Route("")]
        [ResponseType(typeof(ShoppingCart))]
        [CheckPermission(Permission = PredefinedPermissions.Update)]
        public IHttpActionResult Update(ShoppingCart cart)
        {
            _shoppingCartService.SaveChanges(new[] { cart });
            var retVal = _shoppingCartService.GetByIds(new[] { cart.Id }).FirstOrDefault();
            return Ok(retVal);
        }


        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <param name="ids">Array of shopping cart ids</param>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.Delete)]
        public IHttpActionResult DeleteCarts([FromUri] string[] ids)
        {        
            _shoppingCartService.Delete(ids);
            _cacheManager.ClearRegion(ShoppingCartBuilderImpl.CartCacheRegion);
            return StatusCode(HttpStatusCode.NoContent);
        }
              
        private static string GetAsyncLockCartKey(string cartId)
        {
            return "Cart:" + cartId;
        }

    }
}
