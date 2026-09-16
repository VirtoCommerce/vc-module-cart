using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.JsonConverters;
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
    ///  3. Re-writing the id the getter currently returns is a no-op that keeps EVERY target, so a consumer
    ///     re-saving a loaded model does not churn the row.
    ///
    /// Points 1-3 are the IN-PROCESS channel, which is the only one those consumers use. Over JSON the property is
    /// serialized read-only (`SharedWithIdValue`) and ignored on the way in, so no payload can rewrite the set:
    /// one that fills `targets` and leaves the deprecated field null would otherwise delete every target, and one
    /// that sends the field before `targets` would duplicate the first row (Newtonsoft populates rather than
    /// replaces an existing collection). Both are pinned below.
    /// </summary>
#pragma warning disable VC0015 // The obsolete member is the subject under test.
    public class CartSharingSettingTests
    {
        // The platform's REST resolver, so the JSON names below are the ones a real payload carries: it is
        // camelCase, which is what makes the deprecated member and its read-only twin compete for "sharedWithId".
        // The platform additionally deserializes with NullValueHandling.Ignore; these tests deliberately do not,
        // so the model is pinned as safe on its own rather than by a global setting that could be reconfigured.
        private static readonly JsonSerializerSettings PlatformSettings = new() { ContractResolver = new PolymorphJsonContractResolver() };

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

        [Theory]
        // A REST GET/PUT round-trip of a multi-target share.
        [InlineData(@"{""targets"":[{""sharedWithId"":""org-1""},{""sharedWithId"":""org-2""}],""sharedWithId"":""org-1""}")]
        // A client generated against THIS schema: it fills targets and leaves the deprecated field unset, so the
        // two arrive together and the field is null. Honouring it would delete every target the payload just sent.
        [InlineData(@"{""targets"":[{""sharedWithId"":""org-1""},{""sharedWithId"":""org-2""}],""sharedWithId"":null}")]
        // Field order is the client's choice, and Newtonsoft populates an existing collection rather than
        // replacing it: through the setter this left a duplicate org-1 row.
        [InlineData(@"{""sharedWithId"":""org-1"",""targets"":[{""sharedWithId"":""org-1""},{""sharedWithId"":""org-2""}]}")]
        public void Json_WhateverTheDeprecatedFieldCarries_TheTargetsAreExactlyWhatWasSent(string json)
        {
            var setting = JsonConvert.DeserializeObject<CartSharingSetting>(json, PlatformSettings);

            Assert.Equal(["org-1", "org-2"], setting.Targets.Select(x => x.SharedWithId));
        }

        [Fact]
        public void Json_WithoutTargets_LeavesThemUntouched()
        {
            // All a client generated against the pre-3.1011 schema can send. Null, not an empty list: the entity
            // mapping reads null as "this payload says nothing about the targets" and keeps the stored rows.
            var setting = JsonConvert.DeserializeObject<CartSharingSetting>(@"{""sharedWithId"":""org-9""}", PlatformSettings);

            Assert.Null(setting.Targets);
        }

        [Fact]
        public void Json_StillSerializesSharedWithId()
        {
            // Read-only, but still there: an integration that only displays the share keeps working.
            var setting = new CartSharingSetting { Targets = [Target("org-1"), Target("org-2")] };

            Assert.Contains(@"""sharedWithId"":""org-1""", JsonConvert.SerializeObject(setting, PlatformSettings));
        }

        private static CartSharingSettingTarget Target(string sharedWithId) => new() { SharedWithId = sharedWithId };
    }
#pragma warning restore VC0015
}
