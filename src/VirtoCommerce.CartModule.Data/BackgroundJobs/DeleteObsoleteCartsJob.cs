using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs
{
    /// <summary>
    /// This background job hardly removes previously softly removed carts
    /// </summary>
    public class DeleteObsoleteCartsJob
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly ISettingsManager _settingsManager;
        private readonly ILogger _log;

        public DeleteObsoleteCartsJob(Func<ICartRepository> repositoryFactory, ISettingsManager settingsManager, ILogger<DeleteObsoleteCartsJob> log)
        {
            _repositoryFactory = repositoryFactory;
            _settingsManager = settingsManager;
            _log = log;
        }

        [DisableConcurrentExecution(10)]
        public async Task Process()
        {
            _log.LogTrace("Start processing DeleteObsoleteCartsJob job");

            var delayDays = await _settingsManager.GetValueByDescriptorAsync<int>(ModuleConstants.Settings.General.HardDeleteDelayDays);
            var takeCount = await _settingsManager.GetValueByDescriptorAsync<int>(ModuleConstants.Settings.General.PortionDeleteObsoleteCarts);

            using var repository = _repositoryFactory();

            var query = repository.ShoppingCarts.Where(x => x.IsDeleted);
            if (delayDays > 0)
            {
                var thresholdDate = DateTime.UtcNow.AddDays(-delayDays);
                query = query.Where(x => x.ModifiedDate < thresholdDate);
            }

            var totalCount = query.Count();
            _log.LogTrace($"Total soft deleted: {totalCount}");

            var deleted = 0;

            while (true)
            {
                var cartIds = query
                    .Select(x => x.Id)
                    .Take(takeCount)
                    .ToArray();

                if (!cartIds.Any())
                {
                    break;
                }

                _log.LogTrace($"Do remove portion starting from {deleted} to {deleted + cartIds.Length}");
                await repository.RemoveCartsAsync(cartIds);
                await repository.UnitOfWork.CommitAsync();
                _log.LogTrace("Complete remove portion");

                deleted += cartIds.Length;
            }

            _log.LogTrace("Complete processing DeleteObsoleteCartsJob job");
        }
    }
}
