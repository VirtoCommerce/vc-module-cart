using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Notifications;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Model.Search;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class AbandonedCartResolver : IAbandonedCartResolver
    {
        private readonly INotificationMessageSearchService _notificationMessageSearchService;
        private readonly ISettingsManager _settingsManager;

        public AbandonedCartResolver(INotificationMessageSearchService notificationMessageSearchService, ISettingsManager settingsManager)
        {
            _notificationMessageSearchService = notificationMessageSearchService;
            _settingsManager = settingsManager;
        }

        public async Task<AbandonedCart> ResolveAsync(ShoppingCart cart)
        {
            var result = AbstractTypeFactory<AbandonedCart>.TryCreateInstance();

            var searchNotiicationMessageCriteria = AbstractTypeFactory<NotificationMessageSearchCriteria>.TryCreateInstance();
            searchNotiicationMessageCriteria.ObjectType = nameof(ShoppingCart);
            searchNotiicationMessageCriteria.ObjectIds = new[] { cart.Id };
            var searchMessageResult = await _notificationMessageSearchService.SearchMessageAsync(searchNotiicationMessageCriteria);

            var abandonedCartContext = AbstractTypeFactory<AbandonedCartContext>.TryCreateInstance();
            abandonedCartContext.ShoppingCartId = cart.Id;
            abandonedCartContext.ShoppingCartModifiedDate = cart.ModifiedDate;

            var abandonedConditionTree = AbstractTypeFactory<AbandonedCartConditionTree>.TryCreateInstance();
            var firstCondition = abandonedConditionTree.AvailableChildren.FirstOrDefault(x => x.IsSatisfiedBy(abandonedCartContext));
            if (firstCondition is AbandonedCartCondition abandonedCartCondition)
            {
                result.Status = abandonedCartCondition.Status;
            }

            return result;
        }

        public async Task<AbandonedCart> ResolveAsyncOld(ShoppingCart cart)
        {
            var result = new AbandonedCart
            {
                AbandonedDate = cart.ModifiedDate.GetValueOrDefault()
            };

            var firstEventPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCart1stEvent.Name, (int)ModuleConstants.Settings.General.AbandonedCart1stEvent.DefaultValue);
            var secondEventPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCart2ndEvent.Name, (int)ModuleConstants.Settings.General.AbandonedCart2ndEvent.DefaultValue);
            var dropEventPeriod = _settingsManager.GetValue(ModuleConstants.Settings.General.AbandonedCartDrop.Name, (int)ModuleConstants.Settings.General.AbandonedCartDrop.DefaultValue);
            var now = DateTime.UtcNow;
            
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
                result.AbandonedDate = lastNotification.CreatedDate;
                isSent1stEvent = searchMessageResult.Results.Any(x => x.NotificationType.EqualsInvariant(nameof(Abandon1stNotification)));
                isSent2ndEvent = searchMessageResult.Results.Any(x => x.NotificationType.EqualsInvariant(nameof(Abandon2ndNotification)));
            }

            var sendDateTimeSpan = now - result.AbandonedDate;

            if (!isSent1stEvent && !isSent2ndEvent && sendDateTimeSpan > TimeSpan.FromMinutes(firstEventPeriod)
                    && sendDateTimeSpan < TimeSpan.FromMinutes(firstEventPeriod + secondEventPeriod))
            {
                result.Status = AbandonedCartStatus.AbandonedCart1stEvent;
                result.IsAbandoned = true;
            }

            if (!isSent2ndEvent && ((!isSent1stEvent && sendDateTimeSpan > TimeSpan.FromMinutes(firstEventPeriod + secondEventPeriod)
                    && sendDateTimeSpan < TimeSpan.FromMinutes(firstEventPeriod + secondEventPeriod + dropEventPeriod))
                    || (isSent1stEvent
                    && sendDateTimeSpan > TimeSpan.FromMinutes(secondEventPeriod) && (sendDateTimeSpan < TimeSpan.FromMinutes(secondEventPeriod + dropEventPeriod)))))
            {
                result.Status = AbandonedCartStatus.AbandonedCart2ndEvent;
                result.IsAbandoned = true;
            }

            if ((isSent1stEvent && sendDateTimeSpan > TimeSpan.FromMinutes(secondEventPeriod + dropEventPeriod))
                || (isSent2ndEvent && sendDateTimeSpan > TimeSpan.FromMinutes(dropEventPeriod))
                || (!isSent1stEvent && !isSent2ndEvent && sendDateTimeSpan > TimeSpan.FromMinutes(firstEventPeriod + secondEventPeriod + dropEventPeriod)))
            {
                result.Status = AbandonedCartStatus.AbandonedCartDrop;
                result.IsAbandoned = true;
            }

            return result;
        }
    }
}
