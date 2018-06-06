using System.Web.Http;
using Microsoft.Practices.Unity;
using VirtoCommerce.CartModule.Data.Handlers;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.JsonConverters;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : ModuleBase
    {
        private readonly string _connectionString = ConfigurationHelper.GetConnectionStringValue("VirtoCommerce.Cart") ?? ConfigurationHelper.GetConnectionStringValue("VirtoCommerce");
        private readonly IUnityContainer _container; 

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void SetupDatabase()
        {
            using (var context = new CartRepositoryImpl(_connectionString, _container.Resolve<AuditableInterceptor>()))
            {
                var initializer = new SetupDatabaseInitializer<CartRepositoryImpl, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            var eventHandlerRegistrar = _container.Resolve<IHandlerRegistrar>();

            eventHandlerRegistrar.RegisterHandler<CartChangedEvent>(async (message, token) => await _container.Resolve<DeletePropertyCartChangedEventHandler>().Handle(message));

            _container.RegisterType<ICartRepository>(new InjectionFactory(c => new CartRepositoryImpl(_connectionString, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));

            _container.RegisterType<IShoppingCartService, ShoppingCartServiceImpl>();
            _container.RegisterType<IShoppingCartSearchService, ShoppingCartServiceImpl>();

            _container.RegisterType<IShoppingCartBuilder, ShoppingCartBuilderImpl>();
            _container.RegisterType<IShopingCartTotalsCalculator, DefaultShopingCartTotalsCalculator>(new ContainerControlledLifetimeManager());

        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            //Next lines allow to use polymorph types in API controller methods
            var httpConfiguration = _container.Resolve<HttpConfiguration>();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new PolymorphicCartJsonConverter());
        }
    }
}
