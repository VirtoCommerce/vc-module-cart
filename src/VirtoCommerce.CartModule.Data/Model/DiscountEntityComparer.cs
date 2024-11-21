using System.Collections.Generic;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class DiscountEntityComparer : IEqualityComparer<DiscountEntity>
    {
        public bool Equals(DiscountEntity x, DiscountEntity y)
        {
            bool equals;

            if (x != null && y != null)
            {
                equals = x.PromotionId == y.PromotionId &&
                         x.PromotionName == y.PromotionName &&
                         x.PromotionDescription == y.PromotionDescription &&
                         x.CouponCode == y.CouponCode &&
                         x.Currency == y.Currency;
            }
            else
            {
                equals = false;
            }

            return equals;
        }

        public int GetHashCode(DiscountEntity obj)
        {
            var hashCode = 0;

            // Using prime numbers
            hashCode += 17 * obj.PromotionId?.GetHashCode() ?? 19;
            hashCode += 23 * obj.PromotionName?.GetHashCode() ?? 29;
            hashCode += 31 * obj.PromotionDescription?.GetHashCode() ?? 37;
            hashCode += 41 * obj.CouponCode?.GetHashCode() ?? 43;
            hashCode += 47 * obj.Currency?.GetHashCode() ?? 53;

            return hashCode;
        }
    }
}
