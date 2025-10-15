using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model.Search
{
    public class ShoppingCartSearchCriteria : SearchCriteriaBase
    {
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string StoreId { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string[] CustomerIds { get; set; }
        public string OrganizationId { get; set; }
        public bool OrganizationIdIsEmpty { get; set; }
        public DateTime? CreatedStartDate { get; set; }
        public DateTime? CreatedEndDate { get; set; }

        public DateTime? ModifiedStartDate { get; set; }
        public DateTime? ModifiedEndDate { get; set; }

        public bool CustomerOrOrganization { get; set; }

        public bool? IsAnonymous { get; set; }
        public bool? HasLineItems { get; set; }

        public string Type
        {
            get
            {
                return Types?.FirstOrDefault();
            }
            set
            {
                Types = !value.IsNullOrEmpty() ? [value] : null;
            }
        }
        public IList<string> Types { get; set; }

        public string NotType
        {
            get
            {
                return NotTypes?.FirstOrDefault();
            }
            set
            {
                NotTypes = !value.IsNullOrEmpty() ? [value] : null;
            }
        }
        public IList<string> NotTypes { get; set; }

        public bool? HasAbandonmentNotification { get; set; }
        public DateTime? AbandonmentNotificationStartDate { get; set; }
        public DateTime? AbandonmentNotificationEndDate { get; set; }

        public string SharingKey { get; set; }
    }
}
