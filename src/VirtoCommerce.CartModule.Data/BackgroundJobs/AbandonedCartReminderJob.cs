using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Notifications;
using VirtoCommerce.CartModule.Core.Services;
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

/// <summary>
/// This background job sends notifications about abandoned carts
/// </summary>
public class AbandonedCartReminderJob
{
    private readonly IStoreSearchService _storeSearchService;
    private readonly IShoppingCartSearchService _shoppingCartSearchService;
    private readonly INotificationSearchService _notificationSearchService;
    private readonly INotificationSender _notificationSender;
    private readonly IMemberService _memberService;
    private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

    public AbandonedCartReminderJob(
        IStoreSearchService storeSearchService,
        IShoppingCartSearchService shoppingCartSearchService,
        INotificationSearchService notificationSearchService,
        INotificationSender notificationSender,
        IMemberService memberService,
        Func<UserManager<ApplicationUser>> userManagerFactory)
    {
        _storeSearchService = storeSearchService;
        _shoppingCartSearchService = shoppingCartSearchService;
        _notificationSearchService = notificationSearchService;
        _notificationSender = notificationSender;
        _memberService = memberService;
        _userManagerFactory = userManagerFactory;
    }

    [DisableConcurrentExecution(10)]
    public async Task Process()
    {
        var storeSearchCriteria = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();

        await foreach (var searchResult in _storeSearchService.SearchBatchesNoCloneAsync(storeSearchCriteria))
        {
            var stores = searchResult.Results.Where(s => s.Settings.GetValue<bool>(CartSettings.EnableAbandonedCartReminder)).ToList();

            foreach (var store in stores)
            {
                await ProcessСartsInStore(store);
            }
        }
    }

    private async Task ProcessСartsInStore(Store store)
    {
        var cartSearchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();
        cartSearchCriteria.StoreId = store.Id;
        cartSearchCriteria.NotAnonymous = true;
        cartSearchCriteria.NotEmpty = true;
        cartSearchCriteria.NotWishlist = true;

        var delayHours = store.Settings.GetValue<int>(CartSettings.HoursInAbandonedCart);

        if (delayHours > 0)
        {
            cartSearchCriteria.ModifiedEndDate = DateTime.UtcNow.AddHours(-delayHours);
        }

        await foreach (var searchResult in _shoppingCartSearchService.SearchBatchesNoCloneAsync(cartSearchCriteria))
        {
            foreach (var cart in searchResult.Results)
            {
                await SendNotification(store, cart);
            }
        }
    }

    private async Task SendNotification(Store store, ShoppingCart cart)
    {
        var notification = await _notificationSearchService.GetNotificationAsync<AbandonedCartEmailNotification>(new TenantIdentity(cart.StoreId, nameof(Store)));

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
