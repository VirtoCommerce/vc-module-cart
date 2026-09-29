using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    /// <summary>
    /// A sharing setting is one row per list (its Id is the sharing key) with N target rows and one message.
    /// These tests pin the entity/model conversions the persistence relies on:
    ///
    ///  1. FromModel/ToModel carry the targets and the message both ways.
    ///  2. Patch syncs the targets by Id (keep + patch, remove, add) and never touches the setting's Id.
    ///  3. A model with null Targets leaves the stored targets alone, an empty list clears them
    ///     (the NullCollection convention every other cart child collection follows).
    /// </summary>
    public class CartSharingSettingEntityTests
    {
        [Fact]
        public void FromModel_ToModel_RoundTripsTargetsAndMessage()
        {
            var model = new CartSharingSetting
            {
                Id = "key-1",
                ShoppingCartId = "cart-1",
                Scope = "Customer",
                Access = CartSharingAccess.Read,
                Message = "Have a look",
                Targets =
                [
                    new CartSharingSettingTarget { Id = "t-1", SharedWithId = "org-1" },
                    new CartSharingSettingTarget { Id = "t-2", SharedWithId = "org-2" },
                ],
            };

            var entity = new CartSharingSettingEntity().FromModel(model, new PrimaryKeyResolvingMap());
            var roundTripped = entity.ToModel(new CartSharingSetting());

            Assert.Equal("key-1", roundTripped.Id);
            Assert.Equal("cart-1", roundTripped.ShoppingCartId);
            Assert.Equal("Customer", roundTripped.Scope);
            Assert.Equal(CartSharingAccess.Read, roundTripped.Access);
            Assert.Equal("Have a look", roundTripped.Message);
            Assert.Equal(["org-1", "org-2"], roundTripped.Targets.Select(x => x.SharedWithId).ToArray());
            Assert.Equal(["t-1", "t-2"], roundTripped.Targets.Select(x => x.Id).ToArray());
        }

        [Fact]
        public void Patch_SyncsTargetsByIdAndUpdatesTheMessage()
        {
            var stored = StoredSetting("org-1", "org-2");

            var source = new CartSharingSettingEntity
            {
                Id = "key-1",
                ShoppingCartId = "cart-1",
                Scope = "Customer",
                Access = CartSharingAccess.Read,
                Message = "Updated",
                Targets =
                [
                    new CartSharingSettingTargetEntity { Id = "t-org-1", SharedWithId = "org-1" }, // kept
                    new CartSharingSettingTargetEntity { SharedWithId = "org-3" }, // new, no id yet
                ],
            };

            source.Patch(stored);

            Assert.Equal("key-1", stored.Id);
            Assert.Equal("Updated", stored.Message);
            Assert.Equal(["org-1", "org-3"], stored.Targets.Select(x => x.SharedWithId).OrderBy(x => x).ToArray());
        }

        [Fact]
        public void Patch_NullTargets_LeavesStoredTargetsUntouched()
        {
            var stored = StoredSetting("org-1", "org-2");

            var source = new CartSharingSettingEntity().FromModel(
                new CartSharingSetting { Id = "key-1", Scope = "Customer", Access = CartSharingAccess.Read, Message = "Renamed only" },
                new PrimaryKeyResolvingMap());

            source.Patch(stored);

            Assert.Equal("Renamed only", stored.Message);
            Assert.Equal(2, stored.Targets.Count);
        }

        [Fact]
        public void Patch_EmptyTargets_RemovesAllStoredTargets()
        {
            var stored = StoredSetting("org-1", "org-2");

            var source = new CartSharingSettingEntity().FromModel(
                new CartSharingSetting { Id = "key-1", Scope = "Private", Access = CartSharingAccess.Write, Targets = [] },
                new PrimaryKeyResolvingMap());

            source.Patch(stored);

            Assert.Empty(stored.Targets);
            Assert.Equal("Private", stored.Scope);
        }

        private static CartSharingSettingEntity StoredSetting(params string[] sharedWithIds)
        {
            return new CartSharingSettingEntity
            {
                Id = "key-1",
                ShoppingCartId = "cart-1",
                Scope = "Customer",
                Access = CartSharingAccess.Read,
                Message = "Original",
                Targets = new ObservableCollection<CartSharingSettingTargetEntity>(
                    sharedWithIds.Select(x => new CartSharingSettingTargetEntity { Id = $"t-{x}", CartSharingSettingId = "key-1", SharedWithId = x })),
            };
        }
    }
}
