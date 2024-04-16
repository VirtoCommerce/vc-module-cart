using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ProductWishlistEntity : Entity
    {
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public string OrganizationId { get; set; }
    }
}
