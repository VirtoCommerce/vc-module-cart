using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Notifications;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Model.Search;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Notifications;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs
{
    public class CheckingAbandonedCartJob
    {
        private readonly IShoppingCartSearchService _shoppingCartSearchService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISettingsManager _settingsManager;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;
        private readonly INotificationMessageSearchService _notificationMessageSearchService;

        public CheckingAbandonedCartJob(IShoppingCartSearchService shoppingCartSearchService
            , ISettingsManager settingsManager
            , IShoppingCartService shoppingCartService
            , INotificationSearchService notificationSearchService
            , INotificationSender notificationSender
            , INotificationMessageSearchService notificationMessageSearchService)
        {
            _shoppingCartSearchService = shoppingCartSearchService;
            _settingsManager = settingsManager;
            _shoppingCartService = shoppingCartService;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _notificationMessageSearchService = notificationMessageSearchService; 
        }

        [Queue(JobPriority.Normal)]
        public Task CheckingJob(IJobCancellationToken cancellationToken)
        {
            return CheckingAbandonedCarts();
        }

        private async Task CheckingAbandonedCarts()
        {
            var abandonedPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCart1stEvent.Name, 10);
            var searchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();
            var searchResult = await _shoppingCartSearchService.SearchCartAsync(searchCriteria);

            foreach (var cart in searchResult.Results)
            {
                if (cart.ModifiedDate.HasValue)
                {
                    var modifiedDatePeriod = DateTime.Now - cart.ModifiedDate.Value;

                    if (!cart.IsAbandoned && modifiedDatePeriod > TimeSpan.FromMinutes(abandonedPeriod))
                    {
                        await SetAbandoned(cart);
                    }

                    await CheckAbandonedAndNotifyAsync(cart);
                }
            }
        }

        private async Task SetAbandoned(ShoppingCart cart)
        {
            cart.IsAbandoned = true;
            await _shoppingCartService.SaveChangesAsync(new[] { cart });
        }

        private async Task CheckAbandonedAndNotifyAsync(ShoppingCart cart)
        {
            var modifiedDatePeriod = DateTime.Now - cart.ModifiedDate.Value;
            var firstEventPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCart2ndEvent.Name, 20);
            var secondEventPeriod = firstEventPeriod + _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCart2ndEvent.Name, 20);
            var dropEventPeriod = secondEventPeriod + _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCartDrop.Name, 30);

            if (cart.IsAbandoned)
            {
                var notification = AbstractTypeFactory<Notification>.TryCreateInstance();
                var searchNotiicationMessageCriteria = AbstractTypeFactory<NotificationMessageSearchCriteria>.TryCreateInstance();
                searchNotiicationMessageCriteria.NotificationType = nameof(Abandon1stNotification);
                searchNotiicationMessageCriteria.ObjectType = nameof(ShoppingCart);
                searchNotiicationMessageCriteria.ObjectIds = new[] { cart.Id };
                var searchMessageResult = await _notificationMessageSearchService.SearchMessageAsync(searchNotiicationMessageCriteria);

                if (searchMessageResult.TotalCount > 0)
                {
                    var lastNotification = searchMessageResult.Results.OrderBy(x => x.CreatedDate).LastOrDefault();
                    var lastSendNotiicationTimeSpan = DateTime.Now - lastNotification.CreatedDate;

                    if (lastSendNotiicationTimeSpan > TimeSpan.FromMinutes(secondEventPeriod)
                        && lastSendNotiicationTimeSpan < TimeSpan.FromMinutes(secondEventPeriod))
                    {
                        notification = await _notificationSearchService.GetNotificationAsync<Abandon2ndNotification>();
                        await _notificationSender.SendNotificationAsync(notification);
                    }

                    if (lastSendNotiicationTimeSpan > TimeSpan.FromMinutes(secondEventPeriod))
                    {
                        await _shoppingCartService.DeleteAsync(new[] { cart.Id});
                    }
                }
                else
                {
                    notification = await _notificationSearchService.GetNotificationAsync<Abandon1stNotification>();
                    await _notificationSender.SendNotificationAsync(notification);
                }
            }
        }
    }


    

}
