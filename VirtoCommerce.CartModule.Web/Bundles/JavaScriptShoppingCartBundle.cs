using System.Web.Optimization;

namespace VirtoCommerce.CartModule.Web.Bundles
{
	public class JavaScriptShoppingCartBundle : Bundle
	{
		public JavaScriptShoppingCartBundle(string moduleName, string virtualPath)
			: base(virtualPath, new JavaScriptShoppingCartTransform(moduleName))
		{
		}
	}
}