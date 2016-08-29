using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.CartModule.Web.Model
{
	public class CurrencyFormat
	{
		public string CurrencySymbol { get; set; }

		public string DecimalSeparator { get; set; }

		public string ThousandsSeparator { get; set; }

		public int DecimalDigits { get; set; }

		public bool PrefixWithSymbol { get; set; }
	}
}