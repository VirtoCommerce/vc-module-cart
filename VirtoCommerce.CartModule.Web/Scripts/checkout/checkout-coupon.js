var cartModule = angular.module('virtoCommerce.cartModule');
cartModule.component('vcCheckoutCoupon', {
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
