using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CartModule.Data.Builders;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Web.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.Domain.Order.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Tax.Model;
using ShoppingCart = VirtoCommerce.Domain.Cart.Model.ShoppingCart;

namespace VirtoCommerce.CartModule.Web.Controllers.Api
{
	[RoutePrefix("api/checkout2")]
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class CheckoutController : ApiController
	{
		private readonly ICartBuilder _cartBuilder;
		private readonly ICartValidator _cartValidator;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly ICommerceService _commerceService;

		public CheckoutController(ICartBuilder cartBuilder, ICartValidator cartValidator, ICustomerOrderService customerOrderService, ICommerceService commerceService)
		{
			_cartBuilder = cartBuilder;
			_cartValidator = cartValidator;
			_customerOrderService = customerOrderService;
			_commerceService = commerceService;
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
		[Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/items")]
		[ResponseType(typeof(int))]
		public async Task<IHttpActionResult> AddItemToCart(string storeId, string customerId, string cartName, string currency, string cultureName, AddItemModel addItemModel)
		{
			_cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

			using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
			{
				//todo: var products = _catalogSearchService.GetProducts(new[] { id }, Model.Catalog.ItemResponseGroup.ItemLarge);
				//if (products != null && products.Any())
				//{
				//	_cartBuilder.AddItem(products.First(), quantity);
				//	_cartBuilder.Save();
				//}

				_cartBuilder.AddItem(addItemModel).Save();
			}

			return Ok(_cartBuilder.Cart.Items.Count);
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
		[ResponseType(typeof(ICollection<Data.Model.ShippingRate>))]
		public IHttpActionResult GetAvailableShippingRates(string storeId, string customerId, string cartName, string currency, string cultureName, string shipmentId)
		{
			_cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

			var shippingMethods = _cartBuilder.GetAvailableShippingRates();

			return Ok(shippingMethods);
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
		[ResponseType(typeof(string))]
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
		public async Task<IHttpActionResult> AddOrUpdateCartShipment(string storeId, string customerId, string cartName, string currency, string cultureName, ShipmentUpdateModel shipmentUpdateModel)
		{
			_cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

			using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
			{
				_cartBuilder.AddOrUpdateShipment(shipmentUpdateModel).Save();
			}

			return Ok();
		}

		[HttpPost]
		[Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/payments")]
		[ResponseType(typeof(void))]
		public async Task<IHttpActionResult> AddOrUpdateCartPayment(string storeId, string customerId, string cartName, string currency, string cultureName, PaymentUpdateModel paymentUpdateModel)
		{
			_cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

			using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
			{
				_cartBuilder.AddOrUpdatePayment(paymentUpdateModel).Save();
			}

			return Ok();
		}

		[HttpPost]
		[Route("{storeId}/{customerId}/carts/{cartName}/{currency}/{cultureName}/createorder")]
		[ResponseType(typeof(CreateOrderResult))]
		public async Task<IHttpActionResult> CreateOrder(string storeId, string customerId, string cartName, string currency, string cultureName, CreateOrderModel createOrderModel)
		{
			_cartBuilder.GetOrCreateNewTransientCart(storeId, customerId, cartName, currency, cultureName);

			using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(_cartBuilder.Cart.Id)).LockAsync())
			{
				return Ok(_cartBuilder.CreateOrder(createOrderModel));
			}
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

		private static Country[] GetAllCounries()
		{
			var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
				.Select(GetRegionInfo)
				.Where(r => r != null)
				.ToList();

			var countriesJson = File.ReadAllText(HostingEnvironment.MapPath("~/Modules/VirtoCommerce.Cart/countries.json"));
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