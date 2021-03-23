using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Data.Validation
{
    public class PaymentValidator : AbstractValidator<Payment>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.Currency).NotNull().NotEmpty().MaximumLength(3);
            RuleFor(x => x.PaymentGatewayCode).MaximumLength(64);
            //RuleFor(x => x.Purpose).MaximumLength(1024);
            RuleFor(x => x.TaxType).MaximumLength(64);
            RuleFor(x => x.OuterId).MaximumLength(128);

            RuleFor(x => x.BillingAddress).SetValidator(new AddressValidator());
        }
    }
}
