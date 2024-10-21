using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Core.Services;

public interface IAbandonedCartReminderHandler
{
    Task RemindAboutAbandonedCarts();
}
