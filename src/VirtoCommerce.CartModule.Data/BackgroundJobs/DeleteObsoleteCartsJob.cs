using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.Platform.Core.DistributedLock;
using VirtoCommerce.Platform.Core.Jobs;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

/// <summary>
/// This background job hardly removes previously softly removed carts
/// </summary>
public class DeleteObsoleteCartsJob : IBackgroundJobHandler<DeleteObsoleteCartsJobPayload>
{
    // Replaces Hangfire's [DisableConcurrentExecution(10)]: wait up to 10 seconds for a previous run to finish, then
    // skip this occurrence instead of running a second pass in parallel. Unlike the Hangfire attribute, the lock
    // spans the whole worker fleet, not one Hangfire server.
    private const string LockResource = "cart:job:delete-obsolete-carts";
    private static readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);

    private readonly IDeleteObsoleteCartsHandler _deleteHandler;
    private readonly IDistributedLock _distributedLock;
    private readonly ILogger<DeleteObsoleteCartsJob> _logger;

    public DeleteObsoleteCartsJob(IDeleteObsoleteCartsHandler deleteHandler, IDistributedLock distributedLock, ILogger<DeleteObsoleteCartsJob> logger)
    {
        _deleteHandler = deleteHandler;
        _distributedLock = distributedLock;
        _logger = logger;
    }

    public virtual async Task Execute(DeleteObsoleteCartsJobPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var ran = await _distributedLock.TryExecuteAsync(LockResource, _ => Process(), _lockTimeout, cancellationToken);
        if (!ran)
        {
            _logger.LogInformation("Skipped {Job}: a previous run still holds the lock.", nameof(DeleteObsoleteCartsJob));
        }
    }

    public async Task Process()
    {
        await _deleteHandler.DeleteObsoleteCarts();
    }
}
