using System.Web.Http;
using Microsoft.Practices.Unity;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.JsonConverters;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : ModuleBase
    {
        private const string ConnectionStringName = "VirtoCommerce";
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public override void SetupDatabase()
        {
            using (var context = new CartRepositoryImpl(ConnectionStringName, _container.Resolve<AuditableInterceptor>()))
            {
                var initializer = new SetupDatabaseInitializer<CartRepositoryImpl, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }

        }

        public override void Initialize()
        {
            base.Initialize();

            _container.RegisterType<IEventPublisher<CartChangeEvent>, EventPublisher<CartChangeEvent>>();
            _container.RegisterType<IEventPublisher<CartChangedEvent>, EventPublisher<CartChangedEvent>>();

            _container.RegisterType<ICartRepository>(new InjectionFactory(c => new CartRepositoryImpl(ConnectionStringName, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));

            _container.RegisterType<IShoppingCartService, ShoppingCartServiceImpl>();
            _container.RegisterType<IShoppingCartSearchService, ShoppingCartServiceImpl>();
       
            _container.RegisterType<IShoppingCartBuilder, ShoppingCartBuilderImpl>();
            _container.RegisterType<ICartTotalCalculationService, CartTotalCalculationServiceImpl>(new ContainerControlledLifetimeManager());
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            //Next lines allow to use polymorph types in API controller methods
            var httpConfiguration = _container.Resolve<HttpConfiguration>();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new PolymorphicCartJsonConverter());
        }
        #endregion
    }
}
