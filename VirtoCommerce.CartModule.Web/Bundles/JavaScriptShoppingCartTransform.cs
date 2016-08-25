using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Optimization;

namespace VirtoCommerce.CartModule.Web.Bundles
{
	public class JavaScriptShoppingCartTransform : IBundleTransform
	{
		private readonly string _moduleName;

		public JavaScriptShoppingCartTransform(string moduleName)
		{
			_moduleName = moduleName;
		}

		public void Process(BundleContext context, BundleResponse response)
		{
			var strBundleResponse = new StringBuilder();

			foreach (var file in response.Files)
			{
				if (file.IncludedVirtualPath.EndsWith(".js"))
				{
					var absFile = HttpContext.Current.Server.MapPath(file.IncludedVirtualPath);
					var content = File.ReadAllText(absFile);
					strBundleResponse.Append(content);
				}
			}

			strBundleResponse.AppendFormat(@"angular.module('{0}').run(['$templateCache',function(t){{", _moduleName);

			foreach (var file in response.Files)
			{
				if (!file.IncludedVirtualPath.EndsWith(".js"))
				{
					var absFile = HttpContext.Current.Server.MapPath(file.IncludedVirtualPath);
					var content = File.ReadAllText(absFile).Replace("\r\n", "").Replace("\n", "").Replace("'", "\\'");
					strBundleResponse.AppendFormat(@"t.put('{0}','{1}');", file.VirtualFile.Name, content);
				}
			}

			strBundleResponse.Append(@"}]);");

			response.Files = new List<BundleFile>();
			response.Content = strBundleResponse.ToString();
			response.ContentType = "text/javascript";
		}
	}
}