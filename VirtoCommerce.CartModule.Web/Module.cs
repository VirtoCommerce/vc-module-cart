using System.Web.Http;
using Microsoft.Practices.Unity;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.JsonConverters;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : ModuleBase
    {
        private readonly string _connectionStringName = ConfigurationHelper.GetConnectionStringValue("VirtoCommerce");
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void SetupDatabase()
        {
            using (var context = new CartRepositoryImpl(_connectionStringName, _container.Resolve<AuditableInterceptor>()))
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

            _container.RegisterType<ICartRepository>(new InjectionFactory(c => new CartRepositoryImpl(_connectionStringName, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));

            _container.RegisterType<IShoppingCartService, ShoppingCartServiceImpl>();
            _container.RegisterType<IShoppingCartSearchService, ShoppingCartServiceImpl>();

            _container.RegisterType<IShoppingCartBuilder, ShoppingCartBuilderImpl>();
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
