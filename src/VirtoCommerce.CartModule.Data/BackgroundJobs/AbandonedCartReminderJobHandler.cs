using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.DistributedLock;
using VirtoCommerce.Platform.Core.Jobs;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

/// <summary>
/// Runs <see cref="AbandonedCartReminderJob"/> on the recurring schedule, one run at a time across the worker fleet.
/// </summary>
public class AbandonedCartReminderJobHandler(AbandonedCartReminderJob job, IDistributedLock distributedLock, ILogger<AbandonedCartReminderJobHandler> logger)
    : IBackgroundJobHandler<AbandonedCartReminderJobPayload>
{
    // Replaces Hangfire's [DisableConcurrentExecution(10)]: wait up to 10 seconds for a previous run to finish, then
    // skip this occurrence; the next occurrence picks up the work. Two overlapping runs could both pick up a cart
    // before either stamps AbandonmentNotificationDate and send the customer a duplicate reminder. Unlike the
    // Hangfire attribute, the lock spans the whole worker fleet, not one Hangfire server.
    private const string LockResource = "cart:job:abandoned-cart-reminder";
    private static readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);

    public virtual async Task Execute(AbandonedCartReminderJobPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var ran = await distributedLock.TryExecuteAsync(LockResource, _ => job.Process(), _lockTimeout, cancellationToken);
        if (!ran)
        {
            logger.LogInformation("Skipped sending abandoned cart reminders: a previous run still holds the lock.");
        }
    }
}
