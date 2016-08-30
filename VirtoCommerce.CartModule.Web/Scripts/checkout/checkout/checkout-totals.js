var storefrontApp = angular.module('storefrontApp');

storefrontApp.component('vcCheckoutTotals', {
	templateUrl: "checkout-totals.tpl.html",
	bindings: {
		cart: '=',
		currency: '<',
		showPricesWithTaxes: '<',
		displayOnlyTotal: '<'
	},
	controller: [function () {
		var ctrl = this;
	
	}]
});
