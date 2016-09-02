using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Web.Model
{
	public class Country
	{
		public string Name { get; set; }
		public string Code2 { get; set; }
		public string Code3 { get; set; }
		public CountryRegion[] Regions { get; set; }
		public string RegionType { get; set; }
	}
}
