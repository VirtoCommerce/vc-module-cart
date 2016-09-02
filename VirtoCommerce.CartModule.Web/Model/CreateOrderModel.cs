using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Payment.Model;

namespace VirtoCommerce.CartModule.Web.Model
{
	public class CreateOrderModel
	{
		public BankCardInfo BankCardInfo { get; set; }
	}
}
