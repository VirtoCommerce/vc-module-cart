using System;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Notifications;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.BackgroundJobs;
using VirtoCommerce.CartModule.Data.Handlers;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.JsonConverters;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<ICartRepository, CartRepository>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Cart") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CartDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<Func<ICartRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICartRepository>());
            serviceCollection.AddTransient<IShoppingCartService, ShoppingCartService>();
            serviceCollection.AddTransient<IShoppingCartSearchService, ShoppingCartSearchService>();
            serviceCollection.AddTransient<IShoppingCartTotalsCalculator, DefaultShoppingCartTotalsCalculator>();
            serviceCollection.AddTransient<IShoppingCartBuilder, ShoppingCartBuilder>();

            serviceCollection.AddTransient<CartChangedEventHandler>();
            var providerSnapshot = serviceCollection.BuildServiceProvider();
            var inProcessBus = providerSnapshot.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<CartChangedEvent>(async (message, token) => await providerSnapshot.GetService<CartChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<CartChangeEvent>(async (message, token) => await providerSnapshot.GetService<CartChangedEventHandler>().Handle(message));
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Cart",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicCartJsonConverter());

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<LineItem>();
            dynamicPropertyRegistrar.RegisterType<Payment>();
            dynamicPropertyRegistrar.RegisterType<Shipment>();
            dynamicPropertyRegistrar.RegisterType<ShoppingCart>();

            var notificationRegistr = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            notificationRegistr.RegisterNotification<Abandon1stNotification>().WithTemplates(new EmailNotificationTemplate() {
                Subject = "Please complete your order",
                Body = "Dear Customer,<br>You have not completed your order.Please follow the link bellow to access your abandoned shopping cart and complete your shopping. (link) <br>Please enter the promo code(promo code) we are offering and get a(example, 30 %) discount. <br>Should you have any questions,please don't hesitate to contact our Customer Support team (provide contacts)<br> Sincerely yours,<br>(Company name)",
                IsReadonly = false
            });
            notificationRegistr.RegisterNotification<Abandon2ndNotification>().WithTemplates(new EmailNotificationTemplate()
            {
                Subject = "Last notification to complete the order",
                Body = "Your order is still not completed. Please follow the link bellow to access your shopping cart and complete your shopping (link). Get a (example, 30%) discount using the promo code we are kindly providing. Unless you don't complete your order, your shopping cart will be deleted within x hours (time set by the admin).<br> Should you have any questions,please don't hesitate to contact our Customer Support team (provide contacts)<br> Sincerely yours,<br>(Company name)",
                IsReadonly = false
            });

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<CartDbContext>())
                {
                    dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                    dbContext.Database.EnsureCreated();
                    dbContext.Database.Migrate();
                }
            }

            //RecurringJob.AddOrUpdate<CheckingAbandonedCartJob>(j => j.CheckingJob(JobCancellationToken.Null), "0 * 0 ? * * *");
        }

        public void Uninstall()
        {
        }
    }
}
