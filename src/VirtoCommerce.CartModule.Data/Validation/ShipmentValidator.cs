using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Data.Validation
{
    public class ShipmentValidator : AbstractValidator<Shipment>
    {
        public ShipmentValidator()
        {
            RuleFor(x => x.ShipmentMethodCode).MaximumLength(64);
            RuleFor(x => x.ShipmentMethodOption).MaximumLength(64);
            RuleFor(x => x.FulfillmentCenterId).MaximumLength(64);
            RuleFor(x => x.FulfillmentCenterName).MaximumLength(128);
            RuleFor(x => x.Currency).NotNull().NotEmpty().MaximumLength(3);
            RuleFor(x => x.WeightUnit).MaximumLength(16);
            RuleFor(x => x.TaxType).MaximumLength(64);

            RuleFor(x => x.DeliveryAddress).SetValidator(new AddressValidator());
        }
    }
}
