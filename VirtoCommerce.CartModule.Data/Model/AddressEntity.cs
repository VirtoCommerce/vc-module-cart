using System;
using System.ComponentModel.DataAnnotations;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class AddressEntity : Entity
    {
        [StringLength(32)]
        public string AddressType { get; set; }

        [StringLength(64)]
        public string Organization { get; set; }

        [StringLength(3)]
        public string CountryCode { get; set; }

        [Required]
        [StringLength(64)]
        public string CountryName { get; set; }

        [Required]
        [StringLength(128)]
        public string City { get; set; }

        [StringLength(64)]
        public string PostalCode { get; set; }

        [StringLength(2048)]
        public string Line1 { get; set; }

        [StringLength(2048)]
        public string Line2 { get; set; }

        [StringLength(128)]
        public string RegionId { get; set; }

        [StringLength(128)]
        public string RegionName { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string LastName { get; set; }

        [StringLength(64)]
        public string Phone { get; set; }

        [StringLength(64)]
        public string Email { get; set; }


        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public string PaymentId { get; set; }
        public virtual PaymentEntity Payment { get; set; }


        public virtual Address ToModel(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            address.InjectFrom(this);
            address.AddressType = EnumUtility.SafeParse(AddressType, Domain.Commerce.Model.AddressType.BillingAndShipping);

            return address;
        }

        public virtual AddressEntity FromModel(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            this.InjectFrom(address);

            AddressType = address.AddressType.ToString();

            return this;
        }

        public virtual void Patch(AddressEntity target)
        {
            target.City = City;
            target.CountryCode = CountryCode;
            target.CountryName = CountryName;
            target.Phone = Phone;
            target.PostalCode = PostalCode;
            target.RegionId = RegionId;
            target.RegionName = RegionName;
            target.AddressType = AddressType;
            target.City = City;
            target.Email = Email;
            target.FirstName = FirstName;
            target.LastName = LastName;
            target.Line1 = Line1;
            target.Line2 = Line2;
        }
    }
}
