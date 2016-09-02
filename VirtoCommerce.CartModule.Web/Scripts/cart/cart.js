var cartModule = angular.module('virtoCommerce.cartModule', ['ngAnimate', 'ui.bootstrap', 'ngCookies', 'pascalprecht.translate', 'angular.filter']);

cartModule.config(['$translateProvider', function ($translateProvider) {

	//$translateProvider.useStaticFilesLoader({
	//	prefix: 'http://localhost/admin/Modules/VirtoCommerce.Cart/Scripts/checkout/locales/',
	//	suffix: '.json'
	//});
	//$translateProvider.preferredLanguage('en');
}]);

cartModule.factory('virtoCommerce.cartModule.carts', [function () {
	return {};
}]);

cartModule.component('vcCart', {
	templateUrl: "",
	bindings: {
		name: '@',
		apiKey: '@',
		apiUrl: '@',
		storeId: '@',
		userId: '@',
		currencyCode: '@',
		culture: '@'
	},
	controller: ['virtoCommerce.cartModule.carts', 'virtoCommerce.cartModule.api', function (carts, cartApi) {
		var ctrl = this;
		carts[ctrl.name] = this;

		ctrl.currency = { code: ctrl.currencyCode };
		ctrl.availCountries = [];

		this.reload = function () {
			return wrapLoading(function () {
				return cartApi.getCart(ctrl).then(function (response) {
					angular.extend(ctrl, response.data);
					return ctrl;
				}).then(function (cart) {
					cartApi.getCountries(cart).then(function (response) {
						ctrl.availCountries = response.data;
					});
					cartApi.getCurrencies(cart).then(function (response) {
						ctrl.currency = _.find(response.data, function (x) { return x.code === cart.currencyCode; });
					});
				});
			});
		};

		this.addLineItem = function (lineItem) {
			return wrapLoading(function () {
				if (!lineItem.currency)
				{
					lineItem.currency = ctrl.currency.code;
				}
				return cartApi.addLineItem(ctrl, lineItem).then(function () {
					return this.reload();
				});
			});
		}

		this.buyOnClick = function (product) {
			alert(product);
		};

		this.checkout = function () {
			if (this.checkout) {
				this.checkout.show();
			}
		};

		this.reload();	

		function wrapLoading(func) {
			ctrl.loading = true;
			return func().then(function (result) {
				ctrl.loading = false;
				return result;
			},
			function () {
				ctrl.loading = false;
			});
		}

	}]
});

cartModule.controller('virtoCommerce.cartModule.cartController', ['$scope', 'virtoCommerce.cartModule.carts', function ($scope, carts) {

	$scope.carts = carts;

	//$scope.cancel = function () {
	//	$uibModalInstance.dismiss('cancel');
	//};
}]);
