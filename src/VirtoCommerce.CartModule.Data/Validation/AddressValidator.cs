using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Data.Validation
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Name).MaximumLength(2048);
            RuleFor(x => x.AddressType.ToString()).MaximumLength(64);
            RuleFor(x => x.Organization).MaximumLength(512);
            RuleFor(x => x.CountryCode).MaximumLength(3);
            RuleFor(x => x.CountryName).NotNull().NotEmpty().MaximumLength(128);
            RuleFor(x => x.City).NotNull().NotEmpty().MaximumLength(128);
            RuleFor(x => x.PostalCode).MaximumLength(64);
            RuleFor(x => x.Line1).MaximumLength(2048);
            RuleFor(x => x.Line2).MaximumLength(2048);
            RuleFor(x => x.RegionId).MaximumLength(128);
            RuleFor(x => x.RegionName).MaximumLength(128);
            RuleFor(x => x.FirstName).MaximumLength(128);
            RuleFor(x => x.LastName).MaximumLength(128);
            RuleFor(x => x.Phone).MaximumLength(64);
            RuleFor(x => x.Email).MaximumLength(256);
            RuleFor(x => x.OuterId).MaximumLength(128);
        }
    }
}
