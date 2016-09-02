using System.Web.Optimization;

namespace VirtoCommerce.CartModule.Web.Bundles
{
	public class AngularJavaScriptBundle : Bundle
	{
		public AngularJavaScriptBundle(string moduleName, string virtualPath)
			: base(virtualPath, new AngularJavaScriptTemplateCacheBundleTransform(moduleName))
		{
		}
	}
}