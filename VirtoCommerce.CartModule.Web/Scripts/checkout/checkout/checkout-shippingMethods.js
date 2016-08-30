var storefrontApp = angular.module('storefrontApp');

storefrontApp.component('vcCheckoutShippingMethods', {
	templateUrl: "checkout-shippingMethods.tpl.html",
	require: {
		checkoutStep: '^vcCheckoutWizardStep'
	},
	bindings: {
		shipment: '=',
		currency: '<',
		getAvailShippingMethods: '&',
		onSelectShippingMethod: '&'
	},
	controller: [function () {

		var ctrl = this;
		ctrl.availShippingMethods = [];
		ctrl.selectedMethod = {};
		this.$onInit = function () {
			ctrl.checkoutStep.addComponent(this);
			ctrl.getAvailShippingMethods(ctrl.shipment).then(function (availMethods) {
				ctrl.availShippingMethods = availMethods;
				_.each(ctrl.availShippingMethods, function (x) {
					x.id = getMethodId(x);
				});
				ctrl.selectedMethod = _.find(ctrl.availShippingMethods, function (x) { return ctrl.shipment.shipmentMethodCode == x.shippingMethod.code && ctrl.shipment.shipmentMethodOption == x.optionName });
			});
		};

		this.$onDestroy = function () {
			ctrl.checkoutStep.removeComponent(this);
		};

		function getMethodId(method) {
			var retVal = method.shipmentMethodCode;
			if (method.optionName) {
				retVal += ':' + method.optionName;
			}
			return retVal;
		}

		ctrl.selectMethod = function (method) {
			ctrl.selectedMethod = method;
			ctrl.onSelectShippingMethod({ shippingMethod: method });
		};

		ctrl.validate = function () {
			ctrl.form.$setSubmitted();
			return !ctrl.form.$invalid;
		}
	}]
});
