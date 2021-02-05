using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Handlers;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            //serviceCollection.AddTransient<ICartRepository, CartRepository>();
            serviceCollection.AddTransient<ICartRepository, RedisCartRepository>();
            
            serviceCollection.AddDbContext<CartDbContext>((sp, options) => {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("VirtoCommerce.Cart") ?? configuration.GetConnectionString("VirtoCommerce");
                options.UseSqlServer(connectionString);
                });
            serviceCollection.AddTransient<Func<ICartRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICartRepository>());
            serviceCollection.AddTransient<IShoppingCartService, ShoppingCartService>();
            serviceCollection.AddTransient<IShoppingCartSearchService, ShoppingCartSearchService>();
            serviceCollection.AddTransient<IShoppingCartTotalsCalculator, DefaultShoppingCartTotalsCalculator>();
            serviceCollection.AddTransient<IShoppingCartBuilder, ShoppingCartBuilder>();

            serviceCollection.AddTransient<CartChangedEventHandler>();
            //var providerSnapshot = serviceCollection.BuildServiceProvider();
            
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<CartChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CartChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<CartChangeEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CartChangedEventHandler>().Handle(message));
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Cart",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcNewtonsoftJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicCartJsonConverter());

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<LineItem>();
            dynamicPropertyRegistrar.RegisterType<Payment>();
            dynamicPropertyRegistrar.RegisterType<Shipment>();
            dynamicPropertyRegistrar.RegisterType<ShoppingCart>();

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
        }
    }
}
