using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;

namespace VirtoCommerce.CartModule.Data.SqlServer
{
    public class SqlServerCartRawDatabaseCommand : ICartRawDatabaseCommand
    {
        public Task SoftRemove(CartDbContext dbContext, IList<string> ids)
        {
            return ExecuteStoreQueryAsync(dbContext, "UPDATE \"Cart\" SET \"IsDeleted\"='1' WHERE \"Id\" IN ({0})", ids);
        }

        public async Task<IList<ProductWishlistEntity>> FindWishlistsByProductsAsync(CartDbContext dbContext, string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            var command = BuildFindWishlistsCommand(customerId, organizationId, storeId, productIds);
            if (command == null)
            {
                return Array.Empty<ProductWishlistEntity>();
            }

            return await ExecuteQueryAsync<ProductWishlistEntity>(dbContext, command);
        }

        /// <summary>
        /// Builds the wishlist lookup as one seekable branch per owner, combined with UNION ALL.
        /// Returns null when there is no owner to filter by, meaning the caller must not query.
        /// </summary>
        protected virtual Command? BuildFindWishlistsCommand(string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            var hasCustomer = !string.IsNullOrEmpty(customerId);
            var hasOrganization = !string.IsNullOrEmpty(organizationId);

            // Without an owner there is nothing to scope the query to. Returning a command here
            // would match every wishlist in the database.
            if (!hasCustomer && !hasOrganization)
            {
                return null;
            }

            var command = new Command();
            var storeFilter = string.Empty;

            if (!string.IsNullOrEmpty(storeId))
            {
                storeFilter = " AND c.StoreId = @storeId";
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@storeId", storeId));
            }

            var ownerFilters = new List<string>();

            if (hasCustomer)
            {
                ownerFilters.Add("c.CustomerId = @customerId AND c.OrganizationId IS NULL");
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@customerId", customerId));
            }

            if (hasOrganization)
            {
                ownerFilters.Add("c.OrganizationId = @organizationId");
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@organizationId", organizationId));
            }

            // One statement. The OR across CustomerId/OrganizationId is not SARGable on its own,
            // but with an index on CartLineItem.ProductId the optimizer drives from the line-item
            // side and PK-seeks Cart, so the OR is only ever a residual on a handful of rows.
            // Splitting it into two UNION ALL branches was measured at exactly double the work --
            // CartLineItem scan count 1 -> 2, logical reads 19 -> 38 -- for identical results.
            var ownerFilter = ownerFilters.Count > 1
                ? $"({string.Join(" OR ", ownerFilters)})"
                : ownerFilters[0];

            // INNER JOIN, not LEFT JOIN: the predicates on li.* already discard every
            // null-extended row, so an outer join only constrains the optimizer's join order.
            command.Text = $@"
                  SELECT c.Id, li.ProductId
                  FROM Cart c
                  INNER JOIN CartLineItem li
                  ON c.Id = li.ShoppingCartId
                  WHERE c.IsDeleted = 0 AND c.Type = 'Wishlist'
                  AND li.IsGift = 0
                  AND li.ProductId IN (@productIds){storeFilter}
                  AND {ownerFilter}";

            AddArrayParameters(command, "@productIds", productIds);

            return command;
        }

        protected virtual Task<int> ExecuteStoreQueryAsync(CartDbContext dbContext, string commandTemplate, IEnumerable<string> parameterValues)
        {
            var command = CreateCommand(commandTemplate, parameterValues);
            return dbContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
        }

        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new Microsoft.Data.SqlClient.SqlParameter($"@p{i}", v)).ToArray();
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>().ToList(),
            };
        }

        protected static Microsoft.Data.SqlClient.SqlParameter[] AddArrayParameters<T>(Command cmd, string paramNameRoot, IEnumerable<T> values)
        {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually.
             * Each item in the array will end up being it's own Microsoft.Data.SqlClient.SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
            var parameterNames = new List<string>();
            var paramNbr = 1;
            foreach (var value in values)
            {
                var paramName = $"{paramNameRoot}{paramNbr++}";
                parameterNames.Add(paramName);
                var p = new Microsoft.Data.SqlClient.SqlParameter(paramName, value);
                cmd.Parameters.Add(p);
                parameters.Add(p);
            }
            cmd.Text = cmd.Text.Replace(paramNameRoot, string.Join(",", parameterNames));

            return parameters.ToArray();
        }

        private static Task<List<TEntity>> ExecuteQueryAsync<TEntity>(DbContext dbContext, Command command) where TEntity : class
        {
            return dbContext.Set<TEntity>().FromSqlRaw(command.Text, [.. command.Parameters]).ToListAsync();
        }

        protected class Command
        {
            public string Text { get; set; } = string.Empty;
            public IList<object> Parameters { get; set; } = new List<object>();
        }
    }
}
