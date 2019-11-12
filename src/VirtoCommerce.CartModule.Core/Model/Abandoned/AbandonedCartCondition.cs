using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.CartModule.Core.Model
{
    public abstract class AbandonedCartCondition : CompareConditionBase
    {
        public virtual AbandonedCartStatus Status { get; set; }
    }
}
