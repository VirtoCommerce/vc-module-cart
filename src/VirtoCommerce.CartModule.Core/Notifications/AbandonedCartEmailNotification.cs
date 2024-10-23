using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Notifications;

public class AbandonedCartEmailNotification : EmailNotification
{
    public AbandonedCartEmailNotification()
        : base(nameof(AbandonedCartEmailNotification))
    {
    }

    public virtual ShoppingCart Cart { get; set; }
}
