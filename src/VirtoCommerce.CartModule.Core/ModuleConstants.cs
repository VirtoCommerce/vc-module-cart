using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CartModule.Core
{
    [ExcludeFromCodeCoverage]
    public static class ModuleConstants
    {
        public const string WishlistCartType = "Wishlist";

        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "cart:read";
                public const string Create = "cart:create";
                public const string Access = "cart:access";
                public const string Update = "cart:update";
                public const string Delete = "cart:delete";

                public static string[] AllPermissions = [Read, Create, Access, Update, Delete];
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static readonly SettingDescriptor EnableDeleteObsoleteCarts = new SettingDescriptor
                {
                    Name = "Cart.EnableDeleteObsoleteCarts",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static readonly SettingDescriptor PortionDeleteObsoleteCarts = new SettingDescriptor
                {
                    Name = "Cart.PortionDeleteObsoleteCarts",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.PositiveInteger,
                    DefaultValue = 20
                };

                public static readonly SettingDescriptor MaximumCountPerDeleteObsoleteCartsJobExecution = new SettingDescriptor
                {
                    Name = "Cart.MaximumCountPerDeleteObsoleteCartsJobExecution",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.PositiveInteger,
                    DefaultValue = 1000
                };

                public static readonly SettingDescriptor CronDeleteObsoleteCarts = new SettingDescriptor
                {
                    Name = "Cart.CronDeleteObsoleteCarts",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "0 2 */1 * *"
                };

                public static readonly SettingDescriptor HardDeleteDelayDays = new SettingDescriptor
                {
                    Name = "Cart.HardDeleteDelayDays",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.PositiveInteger,
                    DefaultValue = 0
                };

                public static readonly SettingDescriptor EnableAbandonedCartReminder = new SettingDescriptor
                {
                    Name = "Cart.EnableAbandonedCartReminder",
                    GroupName = "Cart|Abandoned Cart Reminder",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static readonly SettingDescriptor CronAbandonedCartReminder = new SettingDescriptor
                {
                    Name = "Cart.CronAbandonedCartReminder",
                    GroupName = "Cart|Abandoned Cart Reminder",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "0 9 * * *"
                };

                public static readonly SettingDescriptor HoursInAbandonedCart = new SettingDescriptor
                {
                    Name = "Cart.HoursInAbandonedCart",
                    GroupName = "Cart|Abandoned Cart Reminder",
                    ValueType = SettingValueType.PositiveInteger,
                    DefaultValue = 120
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return EnableDeleteObsoleteCarts;
                        yield return PortionDeleteObsoleteCarts;
                        yield return MaximumCountPerDeleteObsoleteCartsJobExecution;
                        yield return CronDeleteObsoleteCarts;
                        yield return HardDeleteDelayDays;
                        yield return EnableAbandonedCartReminder;
                        yield return CronAbandonedCartReminder;
                        yield return HoursInAbandonedCart;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> StoreSettings
            {
                get
                {
                    yield return General.EnableAbandonedCartReminder;
                    yield return General.CronAbandonedCartReminder;
                    yield return General.HoursInAbandonedCart;
                }
            }
        }
    }
}
