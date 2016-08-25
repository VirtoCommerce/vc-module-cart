using System;
using System.Linq;
using System.Web.Optimization;
using Microsoft.Practices.Unity;
using VirtoCommerce.CartModule.Data.Builders;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.Bundles;
using VirtoCommerce.ContentModule.Data.Services;
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
	    private const string CheckoutPath = "~/Modules/VirtoCommerce.Cart/Scripts/checkout/";
		private readonly IUnityContainer _container;
		private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;

		public Module(IUnityContainer container, Func<string, IContentBlobStorageProvider> contentStorageProviderFactory)
        {
            _container = container;
			_contentStorageProviderFactory = contentStorageProviderFactory;
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
			var cssBundle = new Bundle("~/checkout/checkout.css", new CssMinify())
				.IncludeDirectory(CheckoutPath, "*.css", true);
			BundleTable.Bundles.Add(cssBundle);

			var partialBundle = new JavaScriptShoppingCartBundle("storefrontApp", "~/checkout/javaScriptShoppingCart.js")
				.IncludeDirectory(CheckoutPath + "buybutton/", "*.js", true)
				.IncludeDirectory(CheckoutPath + "buybutton/", "*.tpl.html", true)
				.Include(CheckoutPath + "services/cartService.js")
				.IncludeDirectory(CheckoutPath + "checkout/", "*.js", true)
				.IncludeDirectory(CheckoutPath + "checkout/", "*.tpl.html", true);
			BundleTable.Bundles.Add(partialBundle);

			var storageProvider = _contentStorageProviderFactory("Themes/");

			var blobSearchResult = storageProvider.Search("", null);
			foreach (var folder in blobSearchResult.Folders)
			{
				if (folder.Name == "default")
				{
					CreateThemeCheckoutBundle(storageProvider, "default");
				}
				else
				{
					var storeThemes = storageProvider.Search(folder.Name, null);
					foreach (var storeThemeFolder in storeThemes.Folders)
					{
						CreateThemeCheckoutBundle(storageProvider, $"{folder.Name}/{storeThemeFolder.Name}");
					}
				}
			}
		}

		private void CreateThemeCheckoutBundle(IContentBlobStorageProvider storageProvider, string folderName)
		{
			var themeFolders = storageProvider.Search(folderName, null);
			var checkoutThemeFolder = themeFolders.Folders.FirstOrDefault(f => f.Name == "checkout");
			if (checkoutThemeFolder != null)
			{
				var cssBundle = new Bundle($"~/checkout/{folderName}/checkout.css", new CssMinify())
					.IncludeDirectory($"~/App_Data/cms-content/Themes/{folderName}/checkout/", "*.css", true);
				BundleTable.Bundles.Add(cssBundle);

				var bundle = new JavaScriptShoppingCartBundle("storefrontApp", $"~/checkout/{folderName}/javaScriptShoppingCart.js")
					.IncludeDirectory($"~/App_Data/cms-content/Themes/{folderName}/checkout/", "*.js", true)
					.IncludeDirectory($"~/App_Data/cms-content/Themes/{folderName}/checkout/", "*.tpl.html", true);

				BundleTable.Bundles.Add(bundle);
			}
		}

		#endregion
	}
}
