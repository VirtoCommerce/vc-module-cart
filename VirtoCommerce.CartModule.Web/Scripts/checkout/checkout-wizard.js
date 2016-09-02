var cartModule = angular.module('virtoCommerce.cartModule');
cartModule.component('vcCheckoutWizard', {
	transclude: true,
	templateUrl: 'checkout-wizard.tpl.html',
	bindings: {
		wizard: '=',
		loading: '=',
		onFinish: '&?',
		onInitialized: '&?'
	},
	controller: ['$scope', function ($scope) {
		var ctrl = this;
		//ctrl.wizard = ctrl;
		ctrl.wizard.steps = [];	
		ctrl.wizard.goToStep = function (step) {
			if (angular.isString(step))
			{
				step = _.find(ctrl.wizard.steps, function (x) { return x.name == step; });
			}
			if (step && ctrl.wizard.currentStep != step && step.canEnter) {
				if (!step.final) {
					step.isActive = true;
					if (ctrl.wizard.currentStep) {
						ctrl.wizard.currentStep.isActive = false;
					}
					ctrl.wizard.currentStep = step;
				}
				else if (ctrl.onFinish)
				{
					ctrl.onFinish();
				}
			}
		};

		ctrl.wizard.nextStep = function () {
			if (!ctrl.wizard.currentStep.validate || ctrl.wizard.currentStep.validate()) {
				if (ctrl.wizard.currentStep.nextStep) {
					if (ctrl.wizard.currentStep.onNextStep) {
						ctrl.wizard.currentStep.onNextStep();
					}
					ctrl.wizard.goToStep(ctrl.wizard.currentStep.nextStep);
				}			
			}
		};

		ctrl.wizard.prevStep = function () {
			ctrl.wizard.goToStep(ctrl.wizard.currentStep.prevStep);
		};

		function rebuildStepsLinkedList(steps) {
			var nextStep = undefined;
			for (var i = steps.length; i-- > 0;) {
				steps[i].prevStep = undefined;
				steps[i].nextStep = undefined;
				if (nextStep && !steps[i].disabled) {
					nextStep.prevStep = steps[i]
				};				
				if (!steps[i].disabled) {
					steps[i].nextStep = nextStep;
					nextStep = steps[i];
				}
			}		
		};
		
		ctrl.wizard.addStep = function (step) {
			ctrl.wizard.steps.push(step);
			$scope.$watch(function () { return step.disabled; }, function () {
				rebuildStepsLinkedList(ctrl.wizard.steps);
			});
			rebuildStepsLinkedList(ctrl.wizard.steps);
			if (!ctrl.wizard.currentStep)
			{
				ctrl.wizard.goToStep(step);
			}
			if (step.final && ctrl.onInitialized)
			{
				ctrl.onInitialized();
			}
		};

	}]
});
