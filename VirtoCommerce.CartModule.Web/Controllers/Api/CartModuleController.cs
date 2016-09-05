using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CartModule.Data.Builders;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Web.Model;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Order.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.CartModule.Web.Controllers.Api
{
    [RoutePrefix("api/cart")]
    [CheckPermission(Permission = PredefinedPermissions.Query)]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CartModuleController : ApiController
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartSearchService _searchService;
        private readonly IStoreService _storeService;
        private readonly ICartBuilder _cartBuilder;
        private readonly ICartValidator _cartValidator;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICommerceService _commerceService;
        private readonly string _modulePath;

        public CartModuleController(IShoppingCartService shoppingCartService, IShoppingCartSearchService searchService, IStoreService storeService, ICartBuilder cartBuilder, ICartValidator cartValidator, ICustomerOrderService customerOrderService,
                                  ICommerceService commerceService, IModuleCatalog moduleCatalog)
        {
            _shoppingCartService = shoppingCartService;
            _searchService = searchService;
            _storeService = storeService;
            _cartBuilder = cartBuilder;
            _cartValidator = cartValidator;
            _customerOrderService = customerOrderService;
            _commerceService = commerceService;
            _modulePath = moduleCatalog.Modules.OfType<ManifestModuleInfo>().First(x => x.ModuleName == "VirtoCommerce.Cart").FullPhysicalPath;
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/current")]
        [ResponseType(typeof(ShoppingCart))]
        public IHttpActionResult GetCart(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            _cartBuilder.EvaluateTax();
            _cartBuilder.EvaluatePromotions();
            _cartValidator.Validate(_cartBuilder.Cart);

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
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/product")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> AddProductToCart(string storeId, string customerId, string cartName, string currency, string cultureName, [FromUri] string productId, [FromUri] int quantity)
        {
            throw new NotImplementedException();
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
                    _cartBuilder.ChangeItemQuantity(lineItemId, quantity);
                    _cartBuilder.Save();
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
                _cartBuilder.RemoveItem(lineItemId);
                _cartBuilder.Save();
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
                _cartBuilder.Clear();
                _cartBuilder.Save();
            }

            return Ok();
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/shipments/{shipmentId}/shippingmethods")]
        [ResponseType(typeof(ICollection<ShippingRate>))]
        public IHttpActionResult GetAvailableShippingRates(string storeId, string customerId, string cartName, string currency, string cultureName, string shipmentId)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            var shippingRates = _cartBuilder.GetAvailableShippingRates();

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
        [ResponseType(typeof(CreateOrderResult))]
        public async Task<IHttpActionResult> CreateOrder(string storeId, string customerId, string cartName, string currency, string cultureName, CreateOrderModel createOrderModel)
        {
            _cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

            throw new NotImplementedException();

            //using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
            //{
            //    var order = _customerOrderService.CreateByShoppingCart(_cartBuilder.Cart.Id);

            //    //todo: Raise domain event
            //    //_orderPlacedEventPublisher.Publish(new OrderPlacedEvent(order.ToWebModel(WorkContext.AllCurrencies, WorkContext.CurrentLanguage), _cartBuilder.Cart));

            //    _cartBuilder.RemoveCart();

            //    var result = new CreateOrderResult()
            //    {
            //        Order = order
            //    };

            //    var incomingPayment = order.InPayments?.FirstOrDefault();
            //    if (incomingPayment != null)
            //    {
            //        var paymentMethods = _cartBuilder.GetAvailablePaymentMethods();
            //        var paymentMethod = paymentMethods.FirstOrDefault(x => x.Code == incomingPayment.GatewayCode);
            //        if (paymentMethod == null)
            //        {
            //            return BadRequest("An appropriate paymentMethod is not found.");
            //        }

            //        result.PaymentMethodType = paymentMethod.PaymentMethodType;

            //        var context = new ProcessPaymentEvaluationContext
            //        {
            //            Order = order,
            //            Payment = incomingPayment,
            //            Store = _cartBuilder.Store,
            //            BankCardInfo = createOrderModel.BankCardInfo
            //        };
            //        result.ProcessPaymentResult = paymentMethod.ProcessPayment(context);

            //        _customerOrderService.Update(new[] { order });
            //    }

                //return Ok(result);
            //}
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
            var retVal = _shoppingCartService.GetById(id);
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
            var coreCart = cart;
            coreCart = _shoppingCartService.Create(coreCart);
            return Ok(coreCart);
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
            var coreCart = cart;
            _shoppingCartService.Update(new[] { cart });
            var retVal = _shoppingCartService.GetById(coreCart.Id);
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


        [HttpGet]
        [Route("currencies")]
        [ResponseType(typeof(Currency))]
        public IHttpActionResult GetCurrencies()
        {
            var currencies = _commerceService.GetAllCurrencies();
            return Ok(currencies);
        }

        [HttpGet]
        [Route("countries")]
        [ResponseType(typeof(Country[]))]
        public IHttpActionResult GetCountries()
        {
            return Ok(GetAllCounries());
        }

        [HttpGet]
        [Route("countries/{countryCode}/regions")]
        [ResponseType(typeof(CountryRegion[]))]
        public IHttpActionResult GetCountryRegions(string countryCode)
        {
            var country = GetAllCounries().FirstOrDefault(c => c.Code2.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase) || c.Code3.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase));
            if (country != null)
            {
                return Ok(country.Regions);
            }
            return Ok();
        }

        private Country[] GetAllCounries()
        {
            var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(GetRegionInfo)
                .Where(r => r != null)
                .ToList();

            var countriesJson = File.ReadAllText(Path.Combine(_modulePath, "countries.json"));
            var countriesDict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(countriesJson);

            var countries = countriesDict
                .Select(kvp => ParseCountry(kvp, regions))
                .Where(c => c.Code3 != null)
                .ToArray();

            return countries;
        }

        private static Country ParseCountry(KeyValuePair<string, JObject> pair, List<RegionInfo> regions)
        {
            var region = regions.FirstOrDefault(r => string.Equals(r.EnglishName, pair.Key, StringComparison.OrdinalIgnoreCase));

            var country = new Country
            {
                Name = pair.Key,
                Code2 = region != null ? region.TwoLetterISORegionName : string.Empty,
                Code3 = region != null ? region.ThreeLetterISORegionName : string.Empty,
                RegionType = pair.Value["label"] != null ? pair.Value["label"].ToString() : null
            };

            var provinceCodes = pair.Value["province_codes"].ToObject<Dictionary<string, string>>();
            if (provinceCodes != null && provinceCodes.Any())
            {
                country.Regions = provinceCodes
                    .Select(kvp => new CountryRegion { Name = kvp.Key, Code = kvp.Value })
                    .ToArray();
            }

            return country;
        }

        private static RegionInfo GetRegionInfo(CultureInfo culture)
        {
            RegionInfo result = null;

            try
            {
                result = new RegionInfo(culture.LCID);
            }
            catch
            {
                // ignored
            }

            return result;
        }

        private static string GetAsyncLockCartKey(string cartId)
        {
            return "Cart:" + cartId;
        }

        private bool StartsWithCurrencySymbol(CultureInfo culture)
        {
            bool startsWithCurrencySymbol =
                culture.NumberFormat.CurrencyPositivePattern == 0 ||
                culture.NumberFormat.CurrencyPositivePattern == 2;
            return culture.TextInfo.IsRightToLeft ? !startsWithCurrencySymbol : startsWithCurrencySymbol;
        }

    }
}
