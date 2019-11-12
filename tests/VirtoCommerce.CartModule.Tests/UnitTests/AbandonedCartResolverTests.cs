using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.NotificationsModule.Core.Model.Search;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    public class AbandonedCartResolverTests
    {
        private readonly Mock<INotificationMessageSearchService> _notificationMessageSearchServiceMock;
        private readonly Mock<ISettingsManager> _settingsManagerMock;

        public AbandonedCartResolverTests()
        {
            _notificationMessageSearchServiceMock = new Mock<INotificationMessageSearchService>();
            _settingsManagerMock = new Mock<ISettingsManager>();

            foreach (var conditionTree in AbstractTypeFactory<AbandonedCartConditionTree>.TryCreateInstance().Traverse<IConditionTree>(x => x.AvailableChildren))
            {
                AbstractTypeFactory<IConditionTree>.RegisterType(conditionTree.GetType());
            }
        }


        [Theory]
        [ClassData(typeof(AbandonedCartTestData))]
        public async Task Resolve_WhenLessTime1stEvent_ReturnsNoneAbandonedCartAsync(DateTime modifiedDate, AbandonedCartStatus status, NotificationMessageSearchResult notificationMessageSearchResult, string id)
        {
            // Arrange
            var cart = new ShoppingCart
            {
                Id = id,
                ModifiedDate = modifiedDate
            };

            var searchNotiicationMessageCriteria = AbstractTypeFactory<NotificationMessageSearchCriteria>.TryCreateInstance();
            searchNotiicationMessageCriteria.ObjectType = nameof(ShoppingCart);
            searchNotiicationMessageCriteria.ObjectIds = new[] { cart.Id };
            _notificationMessageSearchServiceMock.Setup(x => x.SearchMessageAsync(searchNotiicationMessageCriteria))
                                                 .ReturnsAsync(notificationMessageSearchResult);

            var resolver = GetAbandonedCartResolver();

            // Act
            var result = await resolver.ResolveAsync(cart);

            // Assert
            Assert.Equal(status, result.Status);
        }

        private AbandonedCartResolver GetAbandonedCartResolver()
        {
            var abandonedCart1stEventPeriod = 5;
            _settingsManagerMock.Setup(x => x.GetObjectSettingAsync(ModuleConstants.Settings.General.AbandonedCart1stEvent.Name, null, null))
                .ReturnsAsync(new ObjectSettingEntry { Value = abandonedCart1stEventPeriod });
            var abandonedCart2ndEventPeriod = 5;
            _settingsManagerMock.Setup(x => x.GetObjectSettingAsync(ModuleConstants.Settings.General.AbandonedCart2ndEvent.Name, null, null))
                .ReturnsAsync(new ObjectSettingEntry { Value = abandonedCart2ndEventPeriod });
            var abandonedCartDropPeriod = 10;
            _settingsManagerMock.Setup(x => x.GetObjectSettingAsync(ModuleConstants.Settings.General.AbandonedCartDrop.Name, null, null))
                .ReturnsAsync(new ObjectSettingEntry { Value = abandonedCartDropPeriod });

            AbstractTypeFactory<AbandonedCartConditionTree>.RegisterType<AbandonedCartConditionTree>()
                .WithSetupAction(x => {
                    x.AbandonedCart1stEventPeriod = abandonedCart1stEventPeriod;
                    x.AbandonedCart2ndEventPeriod = abandonedCart2ndEventPeriod;
                    x.AbandonedCartDropPeriod = abandonedCartDropPeriod;
                });

            return new AbandonedCartResolver(_notificationMessageSearchServiceMock.Object, _settingsManagerMock.Object);
        }

        public class AbandonedCartTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { DateTime.UtcNow.AddMinutes(-4), AbandonedCartStatus.None, new NotificationMessageSearchResult(), Guid.NewGuid().ToString() };
                yield return new object[] { DateTime.UtcNow.AddMinutes(-6), AbandonedCartStatus.AbandonedCart1stEvent, new NotificationMessageSearchResult(), Guid.NewGuid().ToString() };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
