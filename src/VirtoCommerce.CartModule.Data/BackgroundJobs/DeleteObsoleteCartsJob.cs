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

            var takeCount = (int)_settingsManager.GetValue(ModuleConstants.Settings.General.PortionDeleteObsoleteCarts.Name, ModuleConstants.Settings.General.PortionDeleteObsoleteCarts.DefaultValue);


            using (var repository = _repositoryFactory())
            {
                var totalSoftDeleted = repository.ShoppingCarts.Count(x => x.IsDeleted);
                _log.LogTrace($"Total soft deleted {totalSoftDeleted}");

                for (var i = 0; i < totalSoftDeleted; i += takeCount)
                {
                    var cartIds = repository.ShoppingCarts.Where(x => x.IsDeleted).Select(x => x.Id).Take(takeCount).ToArray();
                    _log.LogTrace($"Do remove portion starting from {i} to {i + takeCount}");
                    await repository.RemoveCartsAsync(cartIds);
                    await repository.UnitOfWork.CommitAsync();
                    _log.LogTrace($"Complete remove portion");
                }
            }
            _log.LogTrace($"Complete processing DeleteObsoleteCartsJob job");
        }
    }
}
