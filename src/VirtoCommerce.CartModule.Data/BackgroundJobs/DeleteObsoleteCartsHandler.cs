using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs
{
    public class DeleteObsoleteCartsHandler : IDeleteObsoleteCartsHandler
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly ISettingsManager _settingsManager;
        private readonly ILogger _log;

        public DeleteObsoleteCartsHandler(Func<ICartRepository> repositoryFactory, ISettingsManager settingsManager, ILogger<DeleteObsoleteCartsJob> log)
        {
            _repositoryFactory = repositoryFactory;
            _settingsManager = settingsManager;
            _log = log;
        }

        public async Task DeleteObsoleteCarts()
        {
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
            _log.LogTrace("Total soft deleted: {TotalCount}", totalCount);

            for (var i = 0; i < totalCount; i += takeCount)
            {
                var cartIds = query
                    .Select(x => x.Id)
                    .Take(takeCount)
                    .ToArray();

                _log.LogTrace("Do remove portion starting from {Start} to {End}", i, i + cartIds.Length);
                await repository.RemoveCartsAsync(cartIds);
                await repository.UnitOfWork.CommitAsync();
                _log.LogTrace("Complete remove portion");
            }
        }
    }
}
