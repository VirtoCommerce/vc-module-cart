using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    /// <summary>
    /// A wishlist shared with many recipients carries one <see cref="CartSharingSettingTarget"/> row each, so the
    /// rows load only for a caller that asked for them (<see cref="CartResponseGroup.WithSharingTargets"/>). The
    /// reduction below is what keeps that safe: a model that never loaded the set must present it as null - the
    /// "untouched" signal every other narrowed collection uses - or saving that cart would delete every target.
    /// </summary>
    public class ShoppingCartReduceDetailsTests
    {
        [Fact]
        public void Full_IncludesSharingTargets()
        {
            Assert.True(CartResponseGroup.Full.HasFlag(CartResponseGroup.WithSharingTargets));
        }

        [Fact]
        public void ReduceDetails_WithoutSharingTargets_NullsThemRatherThanEmptyingThem()
        {
            var cart = CartWithTargets();

            cart.ReduceDetails(CartResponseGroup.WithLineItems.ToString());

            Assert.Single(cart.SharingSettings);
            Assert.Null(cart.SharingSettings[0].Targets);
        }

        [Fact]
        public void ReduceDetails_WithSharingTargets_KeepsThem()
        {
            var cart = CartWithTargets();

            cart.ReduceDetails((CartResponseGroup.WithLineItems | CartResponseGroup.WithSharingTargets).ToString());

            Assert.Equal(["org-1", "org-2"], cart.SharingSettings[0].Targets.Select(x => x.SharedWithId));
        }

        [Fact]
        public void ReduceDetails_DefaultResponseGroup_KeepsTheSharingSettingItself()
        {
            // The setting carries the sharing key, scope, access and message: one row per list, always loaded.
            var cart = CartWithTargets();

            cart.ReduceDetails(null);

            Assert.Equal("key-1", cart.SharingSettings[0].Id);
            Assert.Equal(2, cart.SharingSettings[0].Targets.Count);
        }

        private static ShoppingCart CartWithTargets()
        {
            return new ShoppingCart
            {
                SharingSettings =
                [
                    new CartSharingSetting
                    {
                        Id = "key-1",
                        Scope = "Customer",
                        Targets =
                        [
                            new CartSharingSettingTarget { SharedWithId = "org-1" },
                            new CartSharingSettingTarget { SharedWithId = "org-2" },
                        ],
                    },
                ],
            };
        }
    }
}
