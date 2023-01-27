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
            _log.LogTrace($"Start processing DeleteObsoleteCartsJob job");

            var ttl = await _settingsManager.GetValueByDescriptorAsync<int>(ModuleConstants.Settings.General.CartHardDeleteDays);
            var takeCount = await _settingsManager.GetValueByDescriptorAsync<int>(ModuleConstants.Settings.General.PortionDeleteObsoleteCarts);

            using (var repository = _repositoryFactory())
            {
                var totalSoftDeleted = 0;
                _log.LogTrace($"Total soft deleted {totalSoftDeleted}");

                while (true)
                {
                    var cartIds = repository.ShoppingCarts.Where(x =>
                            x.IsDeleted && (ttl == 0 || x.ModifiedDate < DateTime.UtcNow.AddDays(-ttl)))
                        .Select(x => x.Id)
                        .Take(takeCount).ToArray();
                    if (cartIds.Any())
                    {
                        _log.LogTrace($"Do remove portion starting from {totalSoftDeleted} to {totalSoftDeleted + cartIds.Length}");
                        await repository.RemoveCartsAsync(cartIds);
                        await repository.UnitOfWork.CommitAsync();
                        _log.LogTrace($"Complete remove portion");
                        totalSoftDeleted += cartIds.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            _log.LogTrace($"Complete processing DeleteObsoleteCartsJob job");
        }
    }
}
