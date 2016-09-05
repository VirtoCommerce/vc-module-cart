using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Optimization;
using Microsoft.Practices.Unity;
using VirtoCommerce.CartModule.Data.Builders;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.Bundles;
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
            _container.RegisterType<IEventPublisher<CartChangeEvent>, EventPublisher<CartChangeEvent>>();

            _container.RegisterType<ICartRepository>(new InjectionFactory(c => new CartRepositoryImpl(ConnectionStringName, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));

            _container.RegisterType<IShoppingCartService, ShoppingCartServiceImpl>();
            _container.RegisterType<IShoppingCartSearchService, ShoppingCartSearchServiceImpl>();

			_container.RegisterType<ICartValidator, CartValidator>();
			_container.RegisterType<ICartBuilder, CartBuilder>();
		}

		public override void PostInitialize()
		{
            var moduleCatalog = _container.Resolve<IModuleCatalog>();
            var cartModule = moduleCatalog.Modules.OfType<ManifestModuleInfo>().FirstOrDefault(x => x.ModuleName == "VirtoCommerce.Cart");
            if(cartModule != null)
            {
                var moduleRelativePath = "~/Modules"  + cartModule.FullPhysicalPath.Replace(HostingEnvironment.MapPath("~/Modules"), string.Empty).Replace("\\", "/");
                var cssBundle = new Bundle("~/styles/vc-shopping-cart", new CssMinify())
                                    .IncludeDirectory(Path.Combine(moduleRelativePath, "Content"), "*.css", true);
                BundleTable.Bundles.Add(cssBundle);

                var partialBundle = new AngularJavaScriptBundle("virtoCommerce.cartModule", "~/scripts/vc-shopping-cart")
                    .IncludeDirectory(Path.Combine(moduleRelativePath, "Scripts/cart"), "*.js", true)
                    .IncludeDirectory(Path.Combine(moduleRelativePath, "Scripts/checkout"), "*.js", true)
                    .IncludeDirectory(Path.Combine(moduleRelativePath, "Scripts/checkout"), "*.tpl.html", true)
                    .Include(Path.Combine(moduleRelativePath, "Scripts/services/cartService.js"));
                BundleTable.Bundles.Add(partialBundle);
            }         
        }

		#endregion
	}
}
