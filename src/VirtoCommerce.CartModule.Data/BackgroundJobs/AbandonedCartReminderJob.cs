using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.CartModule.Core.Services;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

/// <summary>
/// This background job sends notifications about abandoned carts
/// </summary>
public class AbandonedCartReminderJob
{
    private readonly IAbandonedCartReminderHandler _handler;

    public AbandonedCartReminderJob(IAbandonedCartReminderHandler handler)
    {
        _handler = handler;
    }

    [DisableConcurrentExecution(10)]
    public async Task Process()
    {
        await _handler.RemindAboutAbandonedCarts();
    }
}
