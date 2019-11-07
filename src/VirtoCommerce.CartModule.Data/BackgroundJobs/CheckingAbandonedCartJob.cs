using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Options;
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
        private readonly EmailSendingOptions _emailSendingOptions;

        private int _firstEventPeriod;
        private int _secondEventPeriod;
        private int _dropEventPeriod;

        public CheckingAbandonedCartJob(IShoppingCartSearchService shoppingCartSearchService
            , ISettingsManager settingsManager
            , IShoppingCartService shoppingCartService
            , INotificationSearchService notificationSearchService
            , INotificationSender notificationSender
            , INotificationMessageSearchService notificationMessageSearchService
            , IOptions<EmailSendingOptions> emailSendingOptions)
        {
            _shoppingCartSearchService = shoppingCartSearchService;
            _settingsManager = settingsManager;
            _shoppingCartService = shoppingCartService;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _notificationMessageSearchService = notificationMessageSearchService;
            _emailSendingOptions = emailSendingOptions.Value;
        }

        [Queue(JobPriority.Normal)]
        public Task CheckingJob(IJobCancellationToken cancellationToken)
        {
            return CheckingCarts();
        }

        private async Task CheckingCarts()
        {
            _firstEventPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCart2ndEvent.Name, (int)ModuleConstants.Settings.General.AbandonedCart2ndEvent.DefaultValue);
            _secondEventPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCart2ndEvent.Name, (int)ModuleConstants.Settings.General.AbandonedCart2ndEvent.DefaultValue);
            _dropEventPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCartDrop.Name, (int)ModuleConstants.Settings.General.AbandonedCartDrop.DefaultValue);

            var searchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();
            var searchResult = await _shoppingCartSearchService.SearchCartAsync(searchCriteria);

            await CheckAndSetAbandonedCarts(searchResult.Results.ToArray());
        }

        private async Task CheckAndSetAbandonedCarts(ShoppingCart[] carts)
        {
            var now = DateTime.UtcNow;

            var abandonedCarts = carts.Where(x => x.IsAbandoned || (x.ModifiedDate.HasValue
                                            && (now - x.ModifiedDate.Value) > TimeSpan.FromMinutes(_firstEventPeriod)))
                                      .ToArray();

            foreach (var abandonedCart in abandonedCarts)
            {
                await CheckAbandonedAndNotifyAsync(abandonedCart);
            }
        }

        private async Task CheckAbandonedAndNotifyAsync(ShoppingCart cart)
        {
            
            var now = DateTime.UtcNow;

            var sendDateTimeSpan = now - cart.ModifiedDate.Value;
            var notification = AbstractTypeFactory<Notification>.TryCreateInstance();
            var tenantIdentity = new TenantIdentity(cart.Id, nameof(ShoppingCart));
            var searchNotiicationMessageCriteria = AbstractTypeFactory<NotificationMessageSearchCriteria>.TryCreateInstance();
            searchNotiicationMessageCriteria.ObjectType = nameof(ShoppingCart);
            searchNotiicationMessageCriteria.ObjectIds = new[] { cart.Id };
            var searchMessageResult = await _notificationMessageSearchService.SearchMessageAsync(searchNotiicationMessageCriteria);

            bool isSent1stEvent = false;
            bool isSent2ndEvent = false;
            if (searchMessageResult.TotalCount > 0
                && searchMessageResult.Results.Any(x => x.NotificationType.EqualsInvariant(nameof(Abandon1stNotification))
                    || x.NotificationType.EqualsInvariant(nameof(Abandon2ndNotification))))
            {
                var lastNotification = searchMessageResult.Results.OrderBy(x => x.CreatedDate).LastOrDefault();
                sendDateTimeSpan = now - lastNotification.CreatedDate;
                isSent1stEvent = searchMessageResult.Results.Any(x => x.NotificationType.EqualsInvariant(nameof(Abandon1stNotification)));
                isSent2ndEvent = searchMessageResult.Results.Any(x => x.NotificationType.EqualsInvariant(nameof(Abandon2ndNotification)));
            }

            if (!isSent1stEvent && !isSent2ndEvent && sendDateTimeSpan > TimeSpan.FromMinutes(_firstEventPeriod)
                    && sendDateTimeSpan < TimeSpan.FromMinutes(_firstEventPeriod + _secondEventPeriod))
            {
                notification = await _notificationSearchService.GetNotificationAsync<Abandon1stNotification>(tenantIdentity);
            }

            if (!isSent2ndEvent && ((!isSent1stEvent && sendDateTimeSpan > TimeSpan.FromMinutes(_firstEventPeriod + _secondEventPeriod)
                    && sendDateTimeSpan < TimeSpan.FromMinutes(_firstEventPeriod + _secondEventPeriod + _dropEventPeriod))
                    || (isSent1stEvent
                    && sendDateTimeSpan > TimeSpan.FromMinutes(_secondEventPeriod) && (sendDateTimeSpan < TimeSpan.FromMinutes(_secondEventPeriod + _dropEventPeriod)))))
            {
                notification = await _notificationSearchService.GetNotificationAsync<Abandon2ndNotification>(tenantIdentity);
            }

            if ((isSent1stEvent && sendDateTimeSpan > TimeSpan.FromMinutes(_secondEventPeriod + _dropEventPeriod))
                || (isSent2ndEvent && sendDateTimeSpan > TimeSpan.FromMinutes(_dropEventPeriod))
                || (!isSent1stEvent && !isSent2ndEvent && sendDateTimeSpan > TimeSpan.FromMinutes(_firstEventPeriod + _secondEventPeriod + _dropEventPeriod)))
            {
                await _shoppingCartService.DeleteAsync(new[] { cart.Id });
            }
            else 
            {
                if ((!isSent1stEvent && notification.Type.EqualsInvariant(nameof(Abandon1stNotification)))
                    || (!isSent2ndEvent && notification.Type.EqualsInvariant(nameof(Abandon2ndNotification))))
                {
                    var recipientEmail = GetRecipientEmail(cart);

                    if (!string.IsNullOrEmpty(recipientEmail))
                    {
                        notification.TenantIdentity = tenantIdentity;
                        notification.SetFromToMembers(_emailSendingOptions.DefaultSender, recipientEmail);
                        _notificationSender.ScheduleSendNotification(notification);
                    }
                }

                if (!cart.IsAbandoned)
                {
                    cart.IsAbandoned = true;
                    await _shoppingCartService.SaveChangesAsync(new[] { cart });
                }
            }
        }

        private string GetRecipientEmail(ShoppingCart cart)
        {
            return cart.Shipments.Select(s => s.DeliveryAddress).Select(x => x.Email).FirstOrDefault(x => !string.IsNullOrEmpty(x));
        }
    }


    

}
