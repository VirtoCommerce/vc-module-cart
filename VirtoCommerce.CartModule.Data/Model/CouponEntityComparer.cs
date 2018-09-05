using System.Collections.Generic;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class CouponEntityComparer : IEqualityComparer<CouponEntity>
    {
        public bool Equals(CouponEntity x, CouponEntity y)
        {
            bool equals;

            if (x != null && y != null)
            {
                equals = x.Code == y.Code;
            }
            else
            {
                equals = false;
            }

            return equals;
        }

        public int GetHashCode(CouponEntity obj)
        {
            var hashCode = 0;

            // Using prime numbers
            hashCode += 17 * obj.Code?.GetHashCode() ?? 19;

            return hashCode;
        }
    }
}
