var cartModule = angular.module('virtoCommerce.cartModule');

cartModule.component('vcCheckoutLineItems', {
	templateUrl: "checkout-lineItems.tpl.html",
	bindings: {
		items: '=',
		currency: '<',
		showPricesWithTaxes: '='
	},
	controller: [function () {
		var ctrl = this;	
	}]
});
