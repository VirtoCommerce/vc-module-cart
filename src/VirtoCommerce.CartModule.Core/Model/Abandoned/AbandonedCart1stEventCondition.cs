using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.CartModule.Core.Model
{
    public class AbandonedCart1stEventCondition : AbandonedCartCondition
    {
        public int AbandonedCart1stEventPeriod { get; set; }
        public override string CompareCondition { get; set; } = ConditionOperation.IsGreaterThanOrEqual;
        public override AbandonedCartStatus Status { get; set; } = AbandonedCartStatus.AbandonedCart1stEvent;

        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is AbandonedCartContext abandonedCartContext)
            {
                var now = DateTime.UtcNow;
                if (abandonedCartContext.ShoppingCartModifiedDate.HasValue)
                {
                    var modifiedDateTimeSpan = now - abandonedCartContext.ShoppingCartModifiedDate.Value;
                    result = UseCompareCondition((int)modifiedDateTimeSpan.TotalMinutes, AbandonedCart1stEventPeriod, 0);
                }
            }

            return result;
        }
    }
}
