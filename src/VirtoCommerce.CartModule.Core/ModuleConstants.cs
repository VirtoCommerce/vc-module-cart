using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CartModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "cart:read";
                public const string Create = "cart:create";
                public const string Access = "cart:access";
                public const string Update = "cart:update";
                public const string Delete = "cart:delete";

                public static string[] AllPermissions = new[] { Read, Create, Access, Update, Delete };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static readonly SettingDescriptor AbandonedCart1stEvent = new SettingDescriptor
                {
                    Name = "Cart.AbandonedCart1stEvent",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 10
                };

                public static readonly SettingDescriptor AbandonedCart2ndEvent = new SettingDescriptor
                {
                    Name = "Cart.AbandonedCart2ndEvent",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 10
                };

                public static readonly SettingDescriptor AbandonedCartDrop = new SettingDescriptor
                {
                    Name = "Cart.AbandonedCartDrop",
                    GroupName = "Cart|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 10
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return AbandonedCart1stEvent;
                        yield return AbandonedCart2ndEvent;
                        yield return AbandonedCartDrop;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings;
        }
    }
}
