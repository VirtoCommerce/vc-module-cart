using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Data.Common
{
    public static class CartStringExtension
    {
        public static Tuple<string, string> SplitIntoTuple(this string input, char separator)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var pieces = input.Split(separator);
            return Tuple.Create(pieces.FirstOrDefault(), pieces.Skip(1).FirstOrDefault());
        }
    }
}
