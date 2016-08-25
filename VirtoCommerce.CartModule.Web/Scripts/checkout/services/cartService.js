angular.module('storefrontApp').service('cartService', ['$http', 'config', function ($http, config) {

	function getUrl(cartContext) {
		return config.apiUrl + 'api/checkout2/' + cartContext.storeId + '/' + cartContext.customerId + '/carts/' + cartContext.cartName + '/' + cartContext.currency + '/' + cartContext.cultureName;
	}

	return {
		getCart: function (cartContext) {
			return $http.get(getUrl(cartContext) + '/current?api_key=' + config.apiKey + '&t=' + new Date().getTime());
		},
		getCartItemsCount: function (cartContext) {
			return $http.get(getUrl(cartContext) + '/itemscount?api_key=' + config.apiKey + '&t=' + new Date().getTime());
		},
		addLineItem: function (cartContext, addItemModel) {
			return $http.post(getUrl(cartContext) + '/items?api_key=' + config.apiKey, addItemModel);
		},
		changeLineItem: function (cartContext, lineItemId, quantity) {
			return $http.put(getUrl(cartContext) + '/items?api_key=' + config.apiKey, { lineItemId: lineItemId, quantity: quantity });
		},
		removeLineItem: function (cartContext, lineItemId) {
			return $http.delete(getUrl(cartContext) + '/items?api_key=' + config.apiKey + '&lineItemId=' + lineItemId);
		},
		clearCart: function (cartContext) {
			return $http.post(getUrl(cartContext) + '/clear?api_key=' + config.apiKey);
		},
		getCountries: function () {
			return $http.get(config.apiUrl + 'api/checkout2/countries?api_key=' + config.apiKey + '&t=' + new Date().getTime());
		},
		getCountryRegions: function (countryCode) {
			return $http.get(config.apiUrl + 'api/checkout2/countries/' + countryCode + '/regions?api_key=' + config.apiKey + 't=' + new Date().getTime());
		},
		addCoupon: function (cartContext, couponCode) {
			return $http.post(getUrl(cartContext) + '/coupons/' + couponCode + '?api_key=' + config.apiKey);
		},
		removeCoupon: function (cartContext) {
			return $http.delete(getUrl(cartContext) + '/coupons?api_key=' + config.apiKey);
		},
		addOrUpdateShipment: function (cartContext, shipment) {
			return $http.post(getUrl(cartContext) + '/shipments?api_key=' + config.apiKey, shipment);
		},
		addOrUpdatePayment: function (cartContext, payment) {
			return $http.post(getUrl(cartContext) + '/payments?api_key=' + config.apiKey, payment);
		},
		getAvailableShippingMethods: function (cartContext, shipmentId) {
			return $http.get(getUrl(cartContext) + '/shipments/' + shipmentId + '/shippingmethods?api_key=' + config.apiKey + '&t=' + new Date().getTime());
		},
		getAvailablePaymentMethods: function (cartContext) {
			return $http.get(getUrl(cartContext) + '/paymentmethods?api_key=' + config.apiKey);
		},
		createOrder: function (cartContext, createOrderModel) {
			return $http.post(getUrl(cartContext) + '/createorder?api_key=' + config.apiKey, createOrderModel);
		}
	}
}]);
