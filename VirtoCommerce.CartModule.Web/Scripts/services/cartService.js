angular.module('virtoCommerce.cartModule')
	.service('virtoCommerce.cartModule.api', ['$http', function ($http) {

	function getUrl(cart) {
		return cart.apiUrl + 'api/cart/' + cart.storeId + '/' + cart.customerId + '/carts/' + cart.name + '/' + cart.currencyCode + '/' + cart.culture;
	}

	return {
		getCart: function (cart) {
			return $http.get(getUrl(cart) + '/current?api_key=' + cart.apiKey + '&t=' + new Date().getTime());
		},
		getCartItemsCount: function (cart) {
			return $http.get(getUrl(cart) + '/itemscount?api_key=' + cart.apiKey + '&t=' + new Date().getTime());
		},
		addLineItem: function (cart, lineItem) {
			return $http.post(getUrl(cart) + '/items?api_key=' + cart.apiKey, lineItem);
		},
		addProduct: function (cart, productId, quantity) {
			return $http.post(getUrl(cart) + '/product?api_key=' + cart.apiKey + '&productId=' + productId + '&quantity=' + quantity, lineItem);
		},
		changeLineItem: function (cart, lineItemId, quantity) {
			return $http.put(getUrl(cart) + '/items?api_key=' + cart.apiKey, { lineItemId: lineItemId, quantity: quantity });
		},
		removeLineItem: function (cart, lineItemId) {
			return $http.delete(getUrl(cart) + '/items?api_key=' + cart.apiKey + '&lineItemId=' + lineItemId);
		},
		clearCart: function (cart) {
			return $http.post(getUrl(cart) + '/clear?api_key=' + cart.apiKey);
		},
		getCountries: function (cart) {
			return $http.get(cart.apiUrl + 'api/cart/countries?api_key=' + cart.apiKey + '&t=' + new Date().getTime());
		},
		getCountryRegions: function (cart, countryCode) {
			return $http.get(cart.apiUrl + 'api/cart/countries/' + countryCode + '/regions?api_key=' + cart.apiKey + 't=' + new Date().getTime());
		},
		getCurrencies: function (cart) {
			return $http.get(cart.apiUrl + 'api/cart/currencies?api_key=' + cart.apiKey + '&t=' + new Date().getTime());
		},
		addCoupon: function (cart, couponCode) {
			return $http.post(getUrl(cart) + '/coupons/' + couponCode + '?api_key=' + cart.apiKey);
		},
		removeCoupon: function (cart) {
			return $http.delete(getUrl(cart) + '/coupons?api_key=' + cart.apiKey);
		},
		addOrUpdateShipment: function (cart, shipment) {
			return $http.post(getUrl(cart) + '/shipments?api_key=' + cart.apiKey, shipment);
		},
		addOrUpdatePayment: function (cart, payment) {
			return $http.post(getUrl(cart) + '/payments?api_key=' + cart.apiKey, payment);
		},
		getAvailableShippingMethods: function (cart, shipmentId) {
			return $http.get(getUrl(cart) + '/shipments/' + shipmentId + '/shippingmethods?api_key=' + cart.apiKey + '&t=' + new Date().getTime());
		},
		getAvailablePaymentMethods: function (cart) {
			return $http.get(getUrl(cart) + '/paymentmethods?api_key=' + cart.apiKey);
		},
		createOrder: function (cart, createOrderModel) {
			return $http.post(getUrl(cart) + '/createorder?api_key=' + cart.apiKey, createOrderModel);
		}
	}
}]);
