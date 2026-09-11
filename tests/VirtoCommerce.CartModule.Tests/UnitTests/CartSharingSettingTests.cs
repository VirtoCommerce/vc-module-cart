using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CartModule.Core.Model;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    /// <summary>
    /// The legacy single-target property must keep consumers built against the old model working at runtime
    /// (X-Cart up to 3.1035 and Sales Rep up to 3.1008 read and write CartSharingSetting.SharedWithId):
    ///
    ///  1. Reading returns the first target, or null when there is none.
    ///  2. Writing an id replaces the whole target set with that single id; writing null clears it —
    ///     exactly the single-valued column semantics those consumers were built for.
    ///  3. Re-writing the id the getter currently returns is a no-op that keeps EVERY target: an old storefront
    ///     re-sending the same target on every save does not churn the row, and a GET/PUT round-trip of the cart
    ///     JSON — which carries the targets back as well — does not collapse a multi-target set.
    ///  4. A payload that omits the targets entirely, which is all a client generated against the pre-3.1011
    ///     schema can send, DOES replace the set. That is point 2, not a defect: single-valued in, single target
    ///     out. Only a caller that can see the set is trusted to change it.
    /// </summary>
#pragma warning disable VC0015 // The obsolete member is the subject under test.
    public class CartSharingSettingTests
    {
        [Fact]
        public void SharedWithId_Get_ReturnsFirstTargetOrNull()
        {
            var setting = new CartSharingSetting();
            Assert.Null(setting.SharedWithId);

            setting.Targets = [Target("org-1"), Target("org-2")];
            Assert.Equal("org-1", setting.SharedWithId);
        }

        [Fact]
        public void SharedWithId_Set_ReplacesTheWholeTargetSet()
        {
            var setting = new CartSharingSetting { Id = "key-1", Targets = [Target("org-1"), Target("org-2")] };

            setting.SharedWithId = "org-3";

            var target = Assert.Single(setting.Targets);
            Assert.Equal("org-3", target.SharedWithId);
            Assert.Equal("key-1", target.CartSharingSettingId);
        }

        [Fact]
        public void SharedWithId_SetNullOrEmpty_ClearsTargets()
        {
            var setting = new CartSharingSetting { Targets = [Target("org-1")] };

            setting.SharedWithId = null;
            Assert.Empty(setting.Targets);

            var fresh = new CartSharingSetting();
            fresh.SharedWithId = string.Empty;
            Assert.Empty(fresh.Targets);
        }

        [Fact]
        public void SharedWithId_SetSameOnlyTarget_KeepsTheExistingRow()
        {
            var existing = Target("org-1");
            existing.Id = "t-1";
            var setting = new CartSharingSetting { Targets = [existing] };

            setting.SharedWithId = "ORG-1";

            Assert.Same(existing, Assert.Single(setting.Targets));
            Assert.Equal("t-1", setting.Targets.Single().Id);
        }

        [Fact]
        public void SharedWithId_SetToCurrentFirstTarget_KeepsAllTargets()
        {
            var first = Target("org-1");
            var second = Target("org-2");
            var setting = new CartSharingSetting { Targets = [first, second] };

            setting.SharedWithId = "ORG-1";

            Assert.Equal(2, setting.Targets.Count);
            Assert.Same(first, setting.Targets[0]);
            Assert.Same(second, setting.Targets[1]);
        }

        [Fact]
        public void SharedWithId_JsonCarryingTheTargets_KeepsTheWholeSet()
        {
            // A REST GET/PUT round-trip: the payload carries the targets, so the trailing sharedWithId is the
            // value the getter produced and setting it changes nothing.
            const string Json = @"{""targets"":[{""sharedWithId"":""org-1""},{""sharedWithId"":""org-2""}],""sharedWithId"":""org-1""}";

            var setting = JsonConvert.DeserializeObject<CartSharingSetting>(Json);

            Assert.Equal(["org-1", "org-2"], setting.Targets.Select(x => x.SharedWithId));
        }

        [Fact]
        public void SharedWithId_JsonWithoutTargets_ReplacesTheSet()
        {
            // All a client generated against the pre-3.1011 schema can send: no targets property at all. It gets
            // single-target semantics, which is what this property is for - pinned so the behaviour is a decision
            // rather than a surprise.
            var setting = JsonConvert.DeserializeObject<CartSharingSetting>(@"{""sharedWithId"":""org-9""}");

            Assert.Equal("org-9", Assert.Single(setting.Targets).SharedWithId);
        }

        private static CartSharingSettingTarget Target(string sharedWithId) => new() { SharedWithId = sharedWithId };
    }
#pragma warning restore VC0015
}
