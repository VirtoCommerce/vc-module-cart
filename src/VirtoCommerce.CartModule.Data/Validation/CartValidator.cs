using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Data.Validation
{
    public class CartValidator : AbstractValidator<ShoppingCart>
    {
        public CartValidator()
        {
            RuleFor(x => x.Name).MaximumLength(64);
            RuleFor(x => x.StoreId).NotNull().NotEmpty().MaximumLength(64);
            RuleFor(x => x.ChannelId).MaximumLength(64);
            RuleFor(x => x.CustomerId).NotNull().NotEmpty().MaximumLength(64);
            RuleFor(x => x.CustomerName).MaximumLength(128);
            RuleFor(x => x.OrganizationId).MaximumLength(64);
            RuleFor(x => x.Currency).NotNull().NotEmpty().MaximumLength(3);
            RuleFor(x => x.LanguageCode).MaximumLength(16);
            RuleFor(x => x.Comment).MaximumLength(2048);
            RuleFor(x => x.ValidationType).MaximumLength(64);
            RuleFor(x => x.Status).MaximumLength(64);
            RuleFor(x => x.PurchaseOrderNumber).MaximumLength(128);
            RuleFor(x => x.Type).MaximumLength(64);

            RuleForEach(x => x.Addresses).SetValidator(new AddressValidator());
            RuleForEach(x => x.Payments).SetValidator(new PaymentValidator());
            RuleForEach(x => x.Shipments).SetValidator(new ShipmentValidator());
        }
    }

    public class ShoppingCartsValidator : AbstractValidator<IEnumerable<ShoppingCart>>
    {
        public const string CartsNotSuppliedMessage = "Please ensure the carts were supplied.";

        public ShoppingCartsValidator()
        {
            RuleFor(x => x).NotNull();
            RuleForEach(x => x).SetValidator(new CartValidator());
        }

        protected override bool PreValidate(ValidationContext<IEnumerable<ShoppingCart>> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", CartsNotSuppliedMessage));
                return false;
            }
            return true;
        }
    }
}
