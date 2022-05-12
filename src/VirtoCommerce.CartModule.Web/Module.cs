using System;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.BackgroundJobs;
using VirtoCommerce.CartModule.Data.Handlers;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Hangfire;
using VirtoCommerce.Platform.Hangfire.Extensions;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<CartDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString(ModuleInfo.Id) ?? configuration.GetConnectionString("VirtoCommerce"));
            });

            serviceCollection.AddTransient<ICartRepository, CartRepository>();
            serviceCollection.AddTransient<Func<ICartRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICartRepository>());
            serviceCollection.AddTransient<IShoppingCartService, ShoppingCartService>();
            serviceCollection.AddTransient<IShoppingCartSearchService, ShoppingCartSearchService>();
            serviceCollection.AddTransient<IShoppingCartTotalsCalculator, DefaultShoppingCartTotalsCalculator>();
            serviceCollection.AddTransient<IShoppingCartBuilder, ShoppingCartBuilder>();

            serviceCollection.AddTransient<CartChangedEventHandler>();

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Cart",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<LineItem>();
            dynamicPropertyRegistrar.RegisterType<Payment>();
            dynamicPropertyRegistrar.RegisterType<Shipment>();
            dynamicPropertyRegistrar.RegisterType<ShoppingCart>();

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);


            var recurringJobManager = appBuilder.ApplicationServices.GetService<IRecurringJobManager>();
            var settingsManager = appBuilder.ApplicationServices.GetService<ISettingsManager>();

            recurringJobManager.WatchJobSetting(
                settingsManager,
                new SettingCronJobBuilder()
                    .SetEnablerSetting(ModuleConstants.Settings.General.EnableDeleteObsoleteCarts)
                    .SetCronSetting(ModuleConstants.Settings.General.CronDeleteObsoleteCarts)
                    .ToJob<DeleteObsoleteCartsJob>(x => x.Process())
                    .Build());

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<CartChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CartChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<CartChangeEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CartChangedEventHandler>().Handle(message));

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<CartDbContext>())
                {
                    dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                    dbContext.Database.EnsureCreated();
                    dbContext.Database.Migrate();
                }
            }
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}
