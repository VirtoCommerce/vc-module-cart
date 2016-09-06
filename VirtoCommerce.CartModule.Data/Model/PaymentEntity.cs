using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
	public class PaymentEntity : Entity
	{
		public PaymentEntity()
		{
			Addresses = new NullCollection<AddressEntity>();
		}

		[StringLength(128)]
		public string OuterId { get; set; }
		[Required]
		[StringLength(64)]
		public string Currency { get; set; }
		[StringLength(64)]
		public string PaymentGatewayCode { get; set; }
		[Column(TypeName = "Money")]
		public decimal Amount { get; set; }
		[StringLength(1024)]
		public string Purpose { get; set; }

		public virtual ShoppingCartEntity ShoppingCart { get; set; }
		public string ShoppingCartId { get; set; }

		public virtual ObservableCollection<AddressEntity> Addresses { get; set; }

        public virtual Payment ToModel(Payment payment)
        {
            if (payment == null)
                throw new NullReferenceException("payment");

            payment.InjectFrom(this);
           
            if (!this.Addresses.IsNullOrEmpty())
            {
                payment.BillingAddress = this.Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }
            return payment;
        }

        public virtual PaymentEntity FromModel(Payment payment, PrimaryKeyResolvingMap pkMap)
        {
            if (payment == null)
                throw new NullReferenceException("payment");

            pkMap.AddPair(payment, this);
            this.InjectFrom(payment);
            if (payment.BillingAddress != null)
            {
                this.Addresses = new ObservableCollection<AddressEntity>(new AddressEntity[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(payment.BillingAddress) });
            }         
            return this;
        }

        public virtual void Patch(PaymentEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            target.Amount = this.Amount;
            target.PaymentGatewayCode = this.PaymentGatewayCode;         

            if (!this.Addresses.IsNullCollection())
            {
                this.Addresses.Patch(target.Addresses, new AddressComparer(), (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }
        }
    }
}
