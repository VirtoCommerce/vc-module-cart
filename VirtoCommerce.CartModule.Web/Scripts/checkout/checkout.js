//Call this to register our module to main application
var cartModule = angular.module('virtoCommerce.cartModule');

cartModule.component('vcCheckout', {
	templateUrl: "checkout.tpl.html",
	bindings: {
		name: '=',
		cart: '=',
		showPricesWithTax: '@'
	},
	controller: ['$rootScope', '$window', 'virtoCommerce.cartModule.api', function ($rootScope, $window, cartService) {
		var ctrl = this;
			
		this.checkout = {
			wizard: {},
			email: {},
			paymentMethod: {},
			shipment: {},
			payment: {},
			order: {},
			coupon: {},
			loading: false,
			isValid: false,
			isFinished: false
		};

		this.validateCheckout = function (checkout) {
			checkout.isValid = checkout.payment && angular.isDefined(checkout.payment.paymentGatewayCode);
			if (checkout.isValid && !checkout.billingAddressEqualsShipping) {
				checkout.isValid = checkout.payment.billingAddress && angular.isDefined(checkout.payment.billingAddress);
			}
			if (ctrl.cart && ctrl.cart.hasPhysicalProducts) {
				if (checkout.isValid) {
					checkout.isValid = checkout.shipment && angular.isDefined(checkout.shipment.shipmentMethodCode);
				}
				if (checkout.isValid && checkout.cart.hasPhysicalProducts) {
					checkout.isValid = checkout.shipment.deliveryAddress && angular.isDefined(checkout.shipment.deliveryAddress);
				}
			}
		};

		this.reloadCart = function () {
			return ctrl.cart.reload().then(function (cart) {
				if (!cart || !cart.id) {
					//ctrl.outerRedirect(ctrl.baseUrl + 'cart');
				} else {
					//todo: add hasPhysicalProducts to model
					cart.hasPhysicalProducts = true;
					ctrl.cart = cart;
					ctrl.checkout.email = cart.email;
					ctrl.checkout.coupon = cart.coupon || ctrl.checkout.coupon;
					if (cart.payments.length) {
						ctrl.checkout.payment = cart.payments[0];
						ctrl.checkout.paymentMethod.code = cart.payments[0].paymentGatewayCode;
					}
					if (cart.shipments.length) {
						ctrl.checkout.shipment = cart.shipments[0];
					}
					ctrl.checkout.billingAddressEqualsShipping = !angular.isDefined(ctrl.checkout.payment.billingAddress);
					if (!cart.hasPhysicalProducts) {
						ctrl.checkout.billingAddressEqualsShipping = false;
					}
				}
				ctrl.validateCheckout(ctrl.checkout);

				return cart;
			});
		};

		this.applyCoupon = function (coupon) {
			coupon.processing = true;
			cartService.addCoupon(ctrl.cart, coupon.code).then(function (response) {
				var coupon = response.data;
				coupon.processing = false;
				ctrl.checkout.coupon = coupon;
				if (!coupon.appliedSuccessfully) {
					coupon.errorCode = 'InvalidCouponCode';
				}
				ctrl.reloadCart();
			}, function (response) {
				coupon.processing = false;
			});
		}

		this.removeCoupon = function (coupon) {
			coupon.processing = true;
			cartService.removeCoupon(ctrl.cart).then(function (response) {
				coupon.processing = false;
				ctrl.checkout.coupon = null;
				ctrl.reloadCart();
			}, function (response) {
				coupon.processing = false;
			});
		}

		this.selectPaymentMethod = function (paymentMethod) {
			ctrl.checkout.payment.paymentGatewayCode = paymentMethod.code;
			ctrl.validateCheckout(ctrl.checkout);
		};


		this.getCountryRegions = function (country) {
			return cartService.getCountryRegions(ctrl.cart, country.code3).then(function (response) {
				return response.data;
			});
		};

		this.getAvailShippingMethods = function (shipment) {
			return wrapLoading(function () {
				return cartService.getAvailableShippingMethods(ctrl.cart, shipment.id).then(function (response) {
					return response.data;
				});
			});
		}

		this.getAvailPaymentMethods = function () {
			return wrapLoading(function () {
				return cartService.getAvailablePaymentMethods(ctrl.cart).then(function (response) {
					return response.data;
				});
			});
		};

		this.selectShippingMethod = function (shippingMethod) {
			if (shippingMethod) {
				ctrl.checkout.shipment.shipmentMethodCode = shippingMethod.shippingMethod.code;
				ctrl.checkout.shipment.shipmentMethodOption = shippingMethod.optionName;
			} else {
				ctrl.checkout.shipment.shipmentMethodCode = undefined;
				ctrl.checkout.shipment.shipmentMethodOption = undefined;
			}
			ctrl.updateShipment(ctrl.checkout.shipment);
		};

		ctrl.updateShipment = function (shipment) {
			if (shipment.deliveryAddress) {
				ctrl.checkout.shipment.deliveryAddress.email = ctrl.checkout.email;
				ctrl.checkout.shipment.deliveryAddress.type = 'Shipping';
			};
			return wrapLoading(function () {
				return cartService.addOrUpdateShipment(ctrl.cart, shipment).then(ctrl.reloadCart);
			});
		};

		ctrl.createOrder = function () {
			ctrl.checkout.loading = true;
			updatePayment(ctrl.checkout.payment).then(function () {
				return cartService.createOrder(ctrl.cart, { bancCardInfo: ctrl.checkout.bankCardInfo });
			}).then(function (response) {
				var order = response.data.order;
				ctrl.checkout.order = order;
				ctrl.checkout.isFinished = true;
				//var orderProcessingResult = response.data.orderProcessingResult;
				//handlePostPaymentResult(order, orderProcessingResult);
			});
		}

		function updatePayment(payment) {
			if (ctrl.checkout.billingAddressEqualsShipping) {
				payment.billingAddress = undefined;
			}

			if (payment.billingAddress) {
				payment.billingAddress.email = ctrl.checkout.email;
				payment.billingAddress.type = 'Billing';

			}

			return cartService.addOrUpdatePayment(ctrl.cart, payment);
		}

		function handlePostPaymentResult(order, orderProcessingResult) {
			if (!orderProcessingResult.isSuccess) {
				$rootScope.$broadcast('storefrontError', {
					type: 'error',
					title: ['Error in new order processing: ', orderProcessingResult.error, 'New Payment status: ' + orderProcessingResult.newPaymentStatus].join(' '),
					message: orderProcessingResult.error,
				});
				return;
			}
			if (orderProcessingResult.paymentMethodType == 'PreparedForm' && orderProcessingResult.htmlForm) {
				ctrl.outerRedirect(ctrl.baseUrl + 'cart/checkout/paymentform?orderNumber=' + order.number);
			}
			if (orderProcessingResult.paymentMethodType == 'Standard' || orderProcessingResult.paymentMethodType == 'Unknown') {
				if (!ctrl.customer.HasAccount) {
					ctrl.outerRedirect(ctrl.baseUrl + 'cart/thanks/' + order.number);
				} else {
					ctrl.outerRedirect(ctrl.baseUrl + 'account/order/' + order.number);
				}
			}
			if (orderProcessingResult.paymentMethodType == 'Redirection' && orderProcessingResult.redirectUrl) {
				$window.location.href = orderProcessingResult.redirectUrl;
			}
		}

		function wrapLoading(func) {
			ctrl.checkout.loading = true;
			return func().then(function (result) {
				ctrl.checkout.loading = false;
				return result;
			},
				function () {
					ctrl.checkout.loading = false;
				});
		}

		ctrl.initialize = function () {		
			ctrl.reloadCart().then(function (cart) {
				ctrl.checkout.wizard.goToStep('shipping-address');
			});
		
		};
	}]
});
