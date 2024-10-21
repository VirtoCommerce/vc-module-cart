using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Notifications;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;
using CartSettings = VirtoCommerce.CartModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

public class AbandonedCartReminderHandler : IAbandonedCartReminderHandler
{
    private readonly IStoreSearchService _storeSearchService;
    private readonly INotificationSearchService _notificationSearchService;
    private readonly INotificationSender _notificationSender;
    private readonly IMemberService _memberService;
    private readonly Func<ICartRepository> _cartRepositoryFactory;
    private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

    public AbandonedCartReminderHandler(
        IStoreSearchService storeSearchService,
        INotificationSearchService notificationSearchService,
        INotificationSender notificationSender,
        IMemberService memberService,
        Func<ICartRepository> cartRepositoryFactory,
        Func<UserManager<ApplicationUser>> userManagerFactory)
    {
        _storeSearchService = storeSearchService;
        _notificationSearchService = notificationSearchService;
        _notificationSender = notificationSender;
        _memberService = memberService;
        _cartRepositoryFactory = cartRepositoryFactory;
        _userManagerFactory = userManagerFactory;
    }

    public async Task RemindAboutAbandonedCarts()
    {
        var searchResult = await _storeSearchService.SearchAsync(AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance());
        var stores = searchResult.Results.Where(s => s.Settings.GetValue<bool>(CartSettings.EnableAbandonedCartReminder));

        using var cartRepository = _cartRepositoryFactory();

        foreach (var store in stores)
        {
            var query = cartRepository.ShoppingCarts.Where(cart => cart.StoreId == store.Id);

            query = query.Where(cart =>
                !cart.IsDeleted &&
                !cart.IsAnonymous &&
                cart.Type != ModuleConstants.WishlistCartType &&
                cart.LineItemsCount > 0);

            var delayHours = store.Settings.GetValue<int>(CartSettings.HoursInAbandonedCart);

            if (delayHours > 0)
            {
                var thresholdDate = DateTime.UtcNow.AddHours(-delayHours);
                query = query.Where(x => x.ModifiedDate < thresholdDate);
            }

            query = query.Include(x => x.Items);

            foreach (var cartEntity in query)
            {
                await SendNotification(store, cartEntity);
            }
        }
    }

    private async Task SendNotification(Store store, ShoppingCartEntity cartEntity)
    {
        var notification = await _notificationSearchService.GetNotificationAsync<AbandonedCartEmailNotification>(new TenantIdentity(cartEntity.StoreId, nameof(Store)));
        var cart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();

        cartEntity.ToModel(cart);

        notification.Cart = cart;
        notification.LanguageCode = cart.LanguageCode;
        notification.From = store.EmailWithName;
        notification.To = await GetCustomerEmailAsync(cart.CustomerId);

        if (!string.IsNullOrEmpty(notification.From) && !string.IsNullOrEmpty(notification.To))
        {
            await _notificationSender.ScheduleSendNotificationAsync(notification);
        }
    }

    private async Task<string> GetCustomerEmailAsync(string userId)
    {
        using var userManager = _userManagerFactory();
        var user = await userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var member = await _memberService.GetByIdAsync(user.MemberId);

            return member?.Emails?.FirstOrDefault() ?? user.Email;
        }

        return null;
    }
}
