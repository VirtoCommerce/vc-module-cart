using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Payment.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
	public class CreateOrderResult
	{
		public PaymentMethodType PaymentMethodType { get; set; }

		public CustomerOrder Order { get; set; }

		public ProcessPaymentResult ProcessPaymentResult { get; set; }
	}
}
