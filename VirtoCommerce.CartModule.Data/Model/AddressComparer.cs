using System.Collections.Generic;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class AddressComparer : IEqualityComparer<AddressEntity>
    {
        public bool Equals(AddressEntity x, AddressEntity y)
        {
            bool equals;

            if (x != null && y != null)
            {
                equals = x.AddressType == y.AddressType &&
                         x.Organization == y.Organization &&
                         x.CountryCode == y.CountryCode &&
                         x.CountryName == y.CountryName &&
                         x.City == y.City &&
                         x.PostalCode == y.PostalCode &&
                         x.Line1 == y.Line1 &&
                         x.Line2 == y.Line2 &&
                         x.RegionId == y.RegionId &&
                         x.RegionName == y.RegionName &&
                         x.FirstName == y.FirstName &&
                         x.LastName == y.LastName &&
                         x.Phone == y.Phone &&
                         x.Email == y.Email;
            }
            else
            {
                equals = false;
            }

            return equals;
        }

        public int GetHashCode(AddressEntity obj)
        {
            var hashCode = 0;

            // Using prime numbers
            hashCode += 17 * obj.AddressType?.GetHashCode() ?? 19;
            hashCode += 23 * obj.Organization?.GetHashCode() ?? 29;
            hashCode += 31 * obj.CountryCode?.GetHashCode() ?? 37;
            hashCode += 41 * obj.CountryName?.GetHashCode() ?? 43;
            hashCode += 47 * obj.City?.GetHashCode() ?? 53;
            hashCode += 59 * obj.PostalCode?.GetHashCode() ?? 61;
            hashCode += 67 * obj.Line1?.GetHashCode() ?? 71;
            hashCode += 73 * obj.Line2?.GetHashCode() ?? 79;
            hashCode += 83 * obj.RegionId?.GetHashCode() ?? 89;
            hashCode += 97 * obj.RegionName?.GetHashCode() ?? 101;
            hashCode += 103 * obj.FirstName?.GetHashCode() ?? 107;
            hashCode += 109 * obj.LastName?.GetHashCode() ?? 113;
            hashCode += 127 * obj.Phone?.GetHashCode() ?? 131;
            hashCode += 137 * obj.Email?.GetHashCode() ?? 139;

            return hashCode;
        }
    }
}
