using VirtoCommerce.CartModule.Core.Model.Abandoned;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.CartModule.Core.Model
{
    public class AbandonedCartConditionTree : ConditionTree
    {
        public int AbandonedCart1stEventPeriod { get; set; }
        public int AbandonedCart2ndEventPeriod { get; set; }
        public int AbandonedCartDropPeriod { get; set; }

        public AbandonedCartConditionTree()
        {
            var block = new BlockAbandonedCartCondition()
                .WithAvailConditions(
                    new AbandonedCart1stEventCondition { AbandonedCart1stEventPeriod = AbandonedCart1stEventPeriod }
                );

            WithAvailConditions(block);
        }
    }
}
