using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Notifications;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.BackgroundJobs;
using VirtoCommerce.CartModule.Data.Handlers;
using VirtoCommerce.CartModule.Data.MySql;
using VirtoCommerce.CartModule.Data.PostgreSql;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Data.SqlServer;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.TemplateLoader.FileSystem;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.Platform.Hangfire;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            serviceCollection.AddDbContext<CartDbContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

                switch (databaseProvider)
                {
                    case "MySql":
                        options.UseMySqlDatabase(connectionString, typeof(MySqlDataAssemblyMarker), Configuration);
                        break;
                    case "PostgreSql":
                        options.UsePostgreSqlDatabase(connectionString, typeof(PostgreSqlDataAssemblyMarker), Configuration);
                        break;
                    default:
                        options.UseSqlServerDatabase(connectionString, typeof(SqlServerDataAssemblyMarker), Configuration);
                        break;
                }
            });

            switch (databaseProvider)
            {
                case "MySql":
                    serviceCollection.AddTransient<ICartRawDatabaseCommand, MySqlCartRawDatabaseCommand>();
                    break;
                case "PostgreSql":
                    serviceCollection.AddTransient<ICartRawDatabaseCommand, PostgreSqlCartRawDatabaseCommand>();
                    break;
                default:
                    serviceCollection.AddTransient<ICartRawDatabaseCommand, SqlServerCartRawDatabaseCommand>();
                    break;
            }

            serviceCollection.AddTransient<ICartRepository, CartRepository>();
            serviceCollection.AddTransient<Func<ICartRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICartRepository>());
            serviceCollection.AddTransient<IShoppingCartService, ShoppingCartService>();
            serviceCollection.AddTransient<IShoppingCartSearchService, ShoppingCartSearchService>();
            serviceCollection.AddTransient<IShoppingCartTotalsCalculator, DefaultShoppingCartTotalsCalculator>();
            serviceCollection.AddTransient<IShoppingCartBuilder, ShoppingCartBuilder>();
            serviceCollection.AddTransient<IWishlistService, WishlistService>();
            serviceCollection.AddTransient<IDeleteObsoleteCartsHandler, DeleteObsoleteCartsHandler>();
            serviceCollection.AddTransient<CartChangedEventHandler>();

            serviceCollection.AddTransient<ILineItemService, LineItemService>();
            serviceCollection.AddTransient<ILineItemSearchService, LineItemSearchService>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var serviceProvider = appBuilder.ApplicationServices;

            var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "Cart", ModuleConstants.Security.Permissions.AllPermissions);

            var dynamicPropertyRegistrar = serviceProvider.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<LineItem>();
            dynamicPropertyRegistrar.RegisterType<Payment>();
            dynamicPropertyRegistrar.RegisterType<Shipment>();
            dynamicPropertyRegistrar.RegisterType<ShoppingCart>();

            var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);
            settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.StoreSettings, nameof(Store));

            var recurringJobService = serviceProvider.GetService<IRecurringJobService>();
            recurringJobService.WatchJobSetting(
                new SettingCronJobBuilder()
                    .SetEnablerSetting(ModuleConstants.Settings.General.EnableDeleteObsoleteCarts)
                    .SetCronSetting(ModuleConstants.Settings.General.CronDeleteObsoleteCarts)
                    .ToJob<DeleteObsoleteCartsJob>(x => x.Process())
                    .Build());
            recurringJobService.WatchJobSetting(
                new SettingCronJobBuilder()
                    .SetEnablerSetting(ModuleConstants.Settings.General.EnableAbandonedCartReminder)
                    .SetCronSetting(ModuleConstants.Settings.General.CronAbandonedCartReminder)
                    .ToJob<AbandonedCartReminderJob>(x => x.Process())
                    .Build());

            appBuilder.RegisterEventHandler<CartChangedEvent, CartChangedEventHandler>();
            appBuilder.RegisterEventHandler<CartChangeEvent, CartChangedEventHandler>();

            var notificationRegistrar = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            var defaultTemplatesDirectory = Path.Combine(ModuleInfo.FullPhysicalPath, "NotificationTemplates");
            notificationRegistrar.RegisterNotification<AbandonedCartEmailNotification>().WithTemplatesFromPath(defaultTemplatesDirectory);

            using var serviceScope = serviceProvider.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CartDbContext>();
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            if (databaseProvider == "SqlServer")
            {
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
            }
            dbContext.Database.Migrate();
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}
