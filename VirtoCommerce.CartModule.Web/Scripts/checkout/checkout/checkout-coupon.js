var storefrontApp = angular.module('storefrontApp');

storefrontApp.component('vcCheckoutCoupon', {
	templateUrl: "checkout-coupon.tpl.html",
	bindings: {
		coupon: '=',
		onApplyCoupon: '&',
		onRemoveCoupon: '&'
	},
	controller: [function () {
		var ctrl = this;	
	}]
});
