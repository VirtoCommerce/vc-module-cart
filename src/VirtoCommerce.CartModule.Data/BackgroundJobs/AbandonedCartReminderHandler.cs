using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

public class AbandonedCartReminderHandler : IAbandonedCartReminderHandler
{
    private readonly Func<ICartRepository> _repositoryFactory;
    private readonly ISettingsManager _settingsManager;

    public AbandonedCartReminderHandler(Func<ICartRepository> repositoryFactory, ISettingsManager settingsManager)
    {
        _repositoryFactory = repositoryFactory;
        _settingsManager = settingsManager;
    }

    public async Task RemindAboutAbandonedCarts()
    {
        var delayHours = await _settingsManager.GetValueAsync<int>(ModuleConstants.Settings.General.HoursInAbandonedCart);

        using var repository = _repositoryFactory();

        var query = repository.ShoppingCarts.Where(x => !x.IsDeleted && x.Type != ModuleConstants.WishlistCartType);

        if (delayHours > 0)
        {
            var thresholdDate = DateTime.UtcNow.AddHours(-delayHours);
            query = query.Where(x => x.ModifiedDate < thresholdDate);
        }

        throw new System.NotImplementedException();
    }
}
