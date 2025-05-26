using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.CartModule.Web.Controllers.Api
{
    [Route("api/carts")]
    [Authorize]
    public class CartModuleController(
        IShoppingCartService shoppingCartService,
        IShoppingCartSearchService searchService,
        IShoppingCartBuilder cartBuilder,
        IShoppingCartTotalsCalculator cartTotalsCalculator
        ) : Controller
    {
        [HttpGet]
        [Route("{storeId}/{customerId}/{cartName}/{currency}/{cultureName}/current")]
        public async Task<ActionResult<ShoppingCart>> GetCart(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), string.Join(":", storeId, customerId, cartName, currency))).LockAsync())
            {
                await cartBuilder.GetOrCreateCartAsync(storeId, customerId, cartName, currency, cultureName);
            }
            return Ok(cartBuilder.Cart);
        }

        [HttpGet]
        [Route("{cartId}/itemscount")]
        public async Task<ActionResult<int>> GetCartItemsCount(string cartId)
        {
            var cart = await shoppingCartService.GetNoCloneAsync(cartId, CartResponseGroup.WithLineItems.ToString());
            return Ok(cart?.Items?.Count ?? 0);
        }

        [HttpPost]
        [Route("{cartId}/items")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddItemToCart(string cartId, [FromBody] LineItem lineItem)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.Full.ToString());
                await cartBuilder.TakeCart(cart).AddItem(lineItem).SaveAsync();
            }
            return NoContent();
        }

        [HttpPut]
        [Route("{cartId}/items")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ChangeCartItem(string cartId, [FromQuery] string lineItemId, [FromQuery] int quantity)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.Full.ToString());
                cartBuilder.TakeCart(cart);
                var lineItem = cartBuilder.Cart.Items.FirstOrDefault(i => i.Id == lineItemId);
                if (lineItem != null)
                {
                    await cartBuilder.ChangeItemQuantity(lineItemId, quantity).SaveAsync();
                }
            }

            return NoContent();
        }

        [HttpDelete]
        [Route("{cartId}/items/{lineItemId}")]
        public async Task<ActionResult<int>> RemoveCartItem(string cartId, string lineItemId)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.Full.ToString());
                await cartBuilder.TakeCart(cart).RemoveItem(lineItemId).SaveAsync();
            }
            return Ok(cartBuilder.Cart.Items.Count);
        }

        [HttpDelete]
        [Route("{cartId}/items")]
        public async Task<ActionResult> ClearCart(string cartId)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.Full.ToString());
                await cartBuilder.TakeCart(cart).Clear().SaveAsync();
            }
            return NoContent();
        }

        [HttpPatch]
        [Route("{cartId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> MergeWithCart(string cartId, [FromBody] ShoppingCart otherCart)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.Full.ToString());
                var builder = await cartBuilder.TakeCart(cart).MergeWithCartAsync(otherCart);
                await builder.SaveAsync();
            }
            return NoContent();
        }

        [HttpGet]
        [Route("{cartId}/availshippingrates")]
        public async Task<ActionResult<ICollection<ShippingRate>>> GetAvailableShippingRates(string cartId)
        {
            var cart = await shoppingCartService.GetByIdAsync(cartId, (CartResponseGroup.WithShipments | CartResponseGroup.WithLineItems).ToString());
            var builder = cartBuilder.TakeCart(cart);
            var shippingRates = await builder.GetAvailableShippingRatesAsync();
            return Ok(shippingRates);
        }

        [HttpPost]
        [Route("availshippingrates")]
        public async Task<ActionResult<ICollection<ShippingRate>>> GetAvailableShippingRatesByContext([FromBody] ShippingEvaluationContext context)
        {
            var builder = cartBuilder.TakeCart(context.ShoppingCart);
            var shippingRates = await builder.GetAvailableShippingRatesAsync();
            return Ok(shippingRates);
        }

        [HttpGet]
        [Route("{cartId}/availpaymentmethods")]
        public async Task<ActionResult<ICollection<PaymentMethod>>> GetAvailablePaymentMethods(string cartId)
        {
            var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.WithPayments.ToString());
            var paymentMethods = await cartBuilder.TakeCart(cart).GetAvailablePaymentMethodsAsync();
            return Ok(paymentMethods);
        }

        [HttpPost]
        [Route("{cartId}/coupons/{couponCode}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddCartCoupon(string cartId, string couponCode)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.Default.ToString());
                await cartBuilder.TakeCart(cart).AddCoupon(couponCode).SaveAsync();
            }
            return NoContent();
        }

        [HttpDelete]
        [Route("{cartId}/coupons/{couponCode}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RemoveCartCoupon(string cartId, string couponCode)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.Default.ToString());
                await cartBuilder.TakeCart(cart).RemoveCoupon().SaveAsync();
            }
            return NoContent();
        }

        [HttpPost]
        [Route("{cartId}/shipments")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddOrUpdateCartShipment(string cartId, [FromBody] Shipment shipment)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.WithShipments.ToString());
                await (await cartBuilder.TakeCart(cart).AddOrUpdateShipmentAsync(shipment)).SaveAsync();
            }
            return NoContent();
        }

        [HttpPost]
        [Route("{cartId}/payments")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddOrUpdateCartPayment(string cartId, [FromBody] Payment payment)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.WithPayments.ToString());
                await (await cartBuilder.TakeCart(cart).AddOrUpdatePaymentAsync(payment)).SaveAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <param name="cartId">Shopping cart id</param>
        [HttpGet]
        [Route("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> GetCartById(string cartId)
        {
            var cart = await shoppingCartService.GetNoCloneAsync(cartId, CartResponseGroup.Full.ToString());
            return Ok(cart);
        }

        /// <summary>
        /// Search shopping carts by given criteria
        /// </summary>
        /// <param name="criteria">Shopping cart search criteria</param>
        [HttpPost]
        [Route("search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<ShoppingCartSearchResult>> SearchShoppingCart([FromBody] ShoppingCartSearchCriteria criteria)
        {
            var result = await searchService.SearchNoCloneAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<ShoppingCart>> Create([FromBody] ShoppingCart cart)
        {
            try
            {
                await shoppingCartService.SaveChangesAsync([cart]);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(cart);
        }

        /// <summary>
        /// Update shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        [HttpPut]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<ShoppingCart>> UpdateShoppingCart([FromBody] ShoppingCart cart)
        {
            using (await AsyncLock.GetLockByKey(CacheKey.With(cart.GetType(), cart.Id)).LockAsync())
            {
                try
                {
                    await shoppingCartService.SaveChangesAsync([cart]);
                }
                catch (FluentValidation.ValidationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Ok(cart);
        }

        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <param name="ids">Array of shopping cart ids</param>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteCarts([FromQuery] string[] ids)
        {
            //For performance reasons use soft shopping cart deletion synchronously first
            await shoppingCartService.DeleteAsync(ids, softDelete: true);

            return NoContent();
        }

        /// <summary>
        /// Calculates totals for cart.
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        [HttpPost]
        [Route("recalculate")]
        public ActionResult<ShoppingCart> RecalculateTotals([FromBody] ShoppingCart cart)
        {
            cartTotalsCalculator.CalculateTotals(cart);
            return Ok(cart);
        }

        /// <summary>
        /// Partial update for the specified ShoppingCart by id
        /// </summary>
        /// <param name="id">ShoppingCart id</param>
        /// <param name="patchDocument">JsonPatchDocument object with fields to update</param>
        [HttpPatch]
        [Route("patch/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PatchCart(string id, [FromBody] JsonPatchDocument<ShoppingCart> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), id)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(id);
                if (cart == null)
                {
                    return NotFound();
                }

                patchDocument.ApplyTo(cart, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    await shoppingCartService.SaveChangesAsync([cart]);
                }
                catch (FluentValidation.ValidationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Partial update for the specified ShoppingCart LineItem by id
        /// </summary>
        /// <param name="cartId">ShoppingCart id</param>
        /// <param name="lineItemId">LineItem id</param>
        /// <param name="patchDocument">JsonPatchDocument object with fields to update</param>
        [HttpPatch]
        [Route("patch/{cartId}/items/{lineItemId}")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PatchCartItem(string cartId, string lineItemId, [FromBody] JsonPatchDocument<LineItem> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return NotFound();
                }

                var lineItem = cart.Items.FirstOrDefault(i => i.Id == lineItemId);
                if (lineItem == null)
                {
                    return NotFound();
                }

                patchDocument.ApplyTo(lineItem, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    await shoppingCartService.SaveChangesAsync([cart]);
                }
                catch (FluentValidation.ValidationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Partial update for the specified ShoppingCart Shipment by id
        /// </summary>
        /// <param name="cartId">ShoppingCart id</param>
        /// <param name="shipmentId">Shipment id</param>
        /// <param name="patchDocument">JsonPatchDocument object with fields to update</param>
        [HttpPatch]
        [Route("patch/{cartId}/shipments/{shipmentId}")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PatchCartShipment(string cartId, string shipmentId, [FromBody] JsonPatchDocument<Shipment> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.WithShipments.ToString());
                if (cart == null)
                {
                    return NotFound();
                }

                cartBuilder.TakeCart(cart);

                var shipment = cartBuilder.Cart.Shipments.FirstOrDefault(i => i.Id == shipmentId);
                if (shipment == null)
                {
                    return NotFound();
                }

                patchDocument.ApplyTo(shipment, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await (await cartBuilder.AddOrUpdateShipmentAsync(shipment)).SaveAsync();
            }
            return NoContent();
        }

        /// <summary>
        /// Partial update for the specified ShoppingCart Payment by id
        /// </summary>
        /// <param name="cartId">ShoppingCart id</param>
        /// <param name="paymentId">Payment id</param>
        /// <param name="patchDocument">JsonPatchDocument object with fields to update</param>
        [HttpPatch]
        [Route("patch/{cartId}/payments/{paymentId}")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PatchCartPayment(string cartId, string paymentId, [FromBody] JsonPatchDocument<Payment> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(ShoppingCart), cartId)).LockAsync())
            {
                var cart = await shoppingCartService.GetByIdAsync(cartId, CartResponseGroup.WithPayments.ToString());
                if (cart == null)
                {
                    return NotFound();
                }

                cartBuilder.TakeCart(cart);

                var payment = cartBuilder.Cart.Payments.FirstOrDefault(i => i.Id == paymentId);
                if (payment == null)
                {
                    return NotFound();
                }

                patchDocument.ApplyTo(payment, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await (await cartBuilder.AddOrUpdatePaymentAsync(payment)).SaveAsync();
            }

            return NoContent();
        }
    }
}
