using System.Data.Entity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Extensions
{
    public static class RepositoryExtension
    {
        public static void DisableChangesTracking(this IRepository repository)
        {
            // http://stackoverflow.com/questions/29106477/nullreferenceexception-in-entity-framework-from-trygetcachedrelatedend
            var dbContext = repository as DbContext;
            if (dbContext != null)
            {
                var dbConfiguration = dbContext.Configuration;
                dbConfiguration.ProxyCreationEnabled = false;
                dbConfiguration.AutoDetectChangesEnabled = false;
            }
        }
    }
}
